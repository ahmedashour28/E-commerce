using ECommerce.Api.Entities;

namespace ECommerce.Api.Repositories.Interfaces
{
    public interface ICartRepository
    {
        Task<Cart?> GetCartAsync(string buyerId);
        Task<Cart> CreateCartAsync(Cart cart);
        Task<bool> DeleteCartAsync(string buyerId);
        Task SaveChangesAsync();
    }
}
