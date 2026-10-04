using ECommerce.Api.Data;
using ECommerce.Api.Entities;
using ECommerce.Api.Interceptors;
using ECommerce.Api.Middleware;
using ECommerce.Api.Repositories.Implementations;
using ECommerce.Api.Repositories.Interfaces;
using ECommerce.Api.Services.Implementations;
using ECommerce.Api.Services.Interfaces;
using ECommerce.Api.Settings;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();

try
{
    Log.Information("Starting Web API...");

    var builder = WebApplication.CreateBuilder(args);

    // Serilog configuration
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());

    builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        });

    // fluent validation
    builder.Services.AddFluentValidationAutoValidation();
    builder.Services.AddValidatorsFromAssemblyContaining<Program>();

    builder.Services.AddOpenApi();

    builder.Services.AddScoped<AuditableEntityInterceptor>();

    builder.Services.AddDbContext<ECommerceDbContext>((sp, options) =>
    {
        var interceptor = sp.GetRequiredService<AuditableEntityInterceptor>();
        options.UseSqlServer(builder.Configuration.GetConnectionString("ECommerceDbConnection")).AddInterceptors(interceptor);
    });

    // Configure Identity
    builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
    {
        // Password settings
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequiredLength = 4;

        // Lockout settings
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
        options.Lockout.MaxFailedAccessAttempts = 5;

        // User settings
        options.User.RequireUniqueEmail = true;
        options.SignIn.RequireConfirmedEmail = true;
    })
    .AddEntityFrameworkStores<ECommerceDbContext>()
    .AddDefaultTokenProviders();

    // authentication and jwt setup
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        var jwtSettings = builder.Configuration.GetSection("Jwt");
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!))
        };
    });

    // Configure Mail Settings
    builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
    builder.Services.AddTransient<IEmailService, EmailService>();

    // Configure Cloudinary Settings
    builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));

    // Configure Stripe Settings
    builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("StripeSettings"));

    // AutoMapper configuration
    builder.Services.AddAutoMapper(typeof(Program));

    // service and repository registrations
    builder.Services.AddScoped<IProductRepository, ProductRepository>();
    builder.Services.AddScoped<IProductService, ProductService>();
    builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
    builder.Services.AddScoped<ICategoryService, CategoryService>();
    builder.Services.AddScoped<IPhotoService, PhotoService>();
    builder.Services.AddScoped<IAuthService, AuthService>();
    builder.Services.AddScoped<ICartRepository, CartRepository>();
    builder.Services.AddScoped<ICartService, CartService>();
    builder.Services.AddScoped<IOrderRepository, OrderRepository>();
    builder.Services.AddScoped<IOrderService, OrderService>();
    builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
    builder.Services.AddScoped<IReviewService, ReviewService>();
    builder.Services.AddScoped<IWishlistRepository, WishlistRepository>();
    builder.Services.AddScoped<IWishlistService, WishlistService>();
    builder.Services.AddScoped<IPaymentService, PaymentService>();

    // Background Services
    builder.Services.AddHostedService<ECommerce.Api.Services.Background.StockReleaseService>();

    // CORS policy
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowLocalClientApp",
            policy =>
            {
                policy.WithOrigins("http://localhost:5173")
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials();
            });
    });

    var app = builder.Build();

    // Seed Database
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            await DbInitializer.SeedAsync(services);
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while seeding the database.");
        }
    }

    app.UseSerilogRequestLogging();

    // Custom middlewares
    app.UseMiddleware<GlobalExceptionMiddleware>();

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }

    app.UseHttpsRedirection();

    app.UseCors("AllowLocalClientApp");

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}