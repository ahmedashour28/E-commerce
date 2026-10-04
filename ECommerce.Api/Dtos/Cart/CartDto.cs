namespace ECommerce.Api.Dtos.Cart
{
    public class CartDto
    {
        public string BuyerId { get; set; } = string.Empty;
        public List<CartItemDto> Items { get; set; } = new();
        public decimal TotalPrice => Items.Sum(item => item.Price * item.Quantity);
    }
}
