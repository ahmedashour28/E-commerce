namespace ECommerce.Api.Dtos.Product
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; } 
        public string CategoryName { get; set; } = string.Empty;
        public string? MainImageUrl { get; set; }
        public List<ProductImageDto> Images { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
