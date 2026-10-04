using ECommerce.Api.Entities;
using Microsoft.AspNetCore.Identity;

namespace ECommerce.Api.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

            // Seed Roles
            await SeedRoleAsync(roleManager, "Admin");
            await SeedRoleAsync(roleManager, "User");

            // Seed Admin User
            var adminUserName = configuration["AdminSettings:UserName"];
            var adminEmail = configuration["AdminSettings:Email"];
            var adminPassword = configuration["AdminSettings:Password"];
            var adminFullName = configuration["AdminSettings:FullName"];

            if (string.IsNullOrEmpty(adminEmail) || string.IsNullOrEmpty(adminPassword))
            {
                logger.LogError("Admin seeding skipped: 'AdminSettings:Email' or 'AdminSettings:Password' is missing in configuration.");
                return;
            }

            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                var newAdmin = new ApplicationUser
                {
                    UserName = adminUserName,
                    Email = adminEmail,
                    FullName = adminFullName,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(newAdmin, adminPassword);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newAdmin, "Admin");
                    logger.LogInformation("Admin user seeded successfully.");
                }
                else
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    logger.LogError($"Failed to seed Admin user. Errors: {errors}");
                }
            }
        }

        private static async Task SeedRoleAsync(RoleManager<ApplicationRole> roleManager, string roleName)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new ApplicationRole { Name = roleName });
            }
        }
    }
}