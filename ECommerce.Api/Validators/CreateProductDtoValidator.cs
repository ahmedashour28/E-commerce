using ECommerce.Api.Dtos.Product;
using FluentValidation;

namespace ECommerce.Api.Validators
{
    public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
    {
        public CreateProductDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Product name is required")
                .Length(2, 100).WithMessage("Name must be between 3 and 100 characters");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than 0")
                .LessThanOrEqualTo(100000).WithMessage("Price must be less than or equal to 100,000");

            RuleFor(x => x.StockQuantity)
                .GreaterThanOrEqualTo(0).WithMessage("Stock quantity cannot be negative");

            RuleFor(x => x.ImageFile)
                .Must(file => file.Length > 0)
                    .WithMessage("File cannot be empty")
                .Must(file => file.Length <= 1 * 1024 * 1024)
                    .WithMessage("File size must be less than 1MB")
                .Must(file => new[] { "image/jpeg", "image/png" }.Contains(file.ContentType))
                    .WithMessage("Only JPEG and PNG files are allowed")
                .When(x => x.ImageFile != null);
        }
    }
}
