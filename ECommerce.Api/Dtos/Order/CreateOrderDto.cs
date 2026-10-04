using ECommerce.Api.Entities.OrderAggregate;

namespace ECommerce.Api.Dtos.Order
{
    public class CreateOrderDto
    {
        public AddressDto ShippingAddress { get; set; } = new();
        public PaymentMethod PaymentMethod { get; set; }
    }
}
