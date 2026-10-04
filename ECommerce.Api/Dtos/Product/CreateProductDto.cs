namespace ECommerce.Api.Dtos.Product
{
    public class CreateProductDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public Guid CategoryId { get; set; }
        public IFormFile? ImageFile { get; set; }
    }
}
