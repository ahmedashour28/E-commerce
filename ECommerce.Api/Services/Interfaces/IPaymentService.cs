using ECommerce.Api.Entities.OrderAggregate;

namespace ECommerce.Api.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<Order> CreateOrUpdatePaymentIntentAsync(Guid orderId);
        Task UpdateOrderPaymentStatus(string paymentIntentId, OrderStatus status);
    }
}
