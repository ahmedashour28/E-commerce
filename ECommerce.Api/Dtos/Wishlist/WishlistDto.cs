namespace ECommerce.Api.Dtos.Wishlist
{
    public class WishlistDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public string Category { get; set; } = string.Empty;
        public DateTime AddedAt { get; set; }
    }
}