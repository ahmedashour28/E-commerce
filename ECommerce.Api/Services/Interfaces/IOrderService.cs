using ECommerce.Api.Dtos.Order;
using ECommerce.Api.Helpers;

namespace ECommerce.Api.Services.Interfaces
{
    public interface IOrderService
    {
        Task<OrderDto> CreateOrderAsync(string buyerId, CreateOrderDto dto);
        Task<IEnumerable<OrderDto>> GetUserOrdersAsync(string buyerId);
        Task<OrderDto?> GetOrderByIdAsync(Guid id, string buyerId);
        Task<PagedList<OrderDto>> GetAllOrdersAsync(OrderParams orderParams);
        Task UpdateOrderStatusAsync(Guid orderId, string status);
    }
}
