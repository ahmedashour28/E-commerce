using ECommerce.Api.Dtos.Wishlist;

namespace ECommerce.Api.Services.Interfaces
{
    public interface IWishlistService
    {
        Task<string> ToggleWishlistAsync(Guid userId, Guid productId);
        Task<IEnumerable<WishlistDto>> GetWishlistAsync(Guid userId);
    }
}