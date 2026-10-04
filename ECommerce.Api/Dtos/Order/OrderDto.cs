namespace ECommerce.Api.Dtos.Order
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public string BuyerId { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public List<OrderItemDto> OrderItems { get; set; } = new();
        public AddressDto ShippingAddress { get; set; } = new();
    }
}
