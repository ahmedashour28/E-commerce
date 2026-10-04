using ECommerce.Api.Entities;

namespace ECommerce.Api.Repositories.Interfaces
{
    public interface IWishlistRepository
    {
        Task<IEnumerable<Wishlist>> GetByUserIdAsync(Guid userId);
        Task<Wishlist?> GetByUserAndProductAsync(Guid userId, Guid productId);
        Task AddAsync(Wishlist wishlist);
        Task DeleteAsync(Wishlist wishlist);
    }
}