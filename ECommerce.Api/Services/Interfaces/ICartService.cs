using ECommerce.Api.Dtos.Cart;

namespace ECommerce.Api.Services.Interfaces
{
    public interface ICartService
    {
        Task<CartDto> GetCartAsync(string buyerId);
        Task<CartDto> AddItemToCartAsync(string buyerId, Guid productId, int quantity);
        Task RemoveItemFromCartAsync(string buyerId, Guid productId, int quantity);
        Task DeleteCartAsync(string buyerId);
    }
}
