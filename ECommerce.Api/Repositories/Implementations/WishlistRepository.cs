using ECommerce.Api.Data;
using ECommerce.Api.Entities;
using ECommerce.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Repositories.Implementations
{
    public class WishlistRepository : IWishlistRepository
    {
        private readonly ECommerceDbContext _context;

        public WishlistRepository(ECommerceDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Wishlist wishlist)
        {
            await _context.Wishlists.AddAsync(wishlist);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Wishlist wishlist)
        {
            _context.Wishlists.Remove(wishlist);
            await _context.SaveChangesAsync();
        }

        public async Task<Wishlist?> GetByUserAndProductAsync(Guid userId, Guid productId)
        {
            return await _context.Wishlists
                .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);
        }

        public async Task<IEnumerable<Wishlist>> GetByUserIdAsync(Guid userId)
        {
            return await _context.Wishlists
                .Include(w => w.Product)
                .ThenInclude(p => p.Category)
                .Include(w => w.Product)
                .ThenInclude(p => p.Images)
                .Where(w => w.UserId == userId)
                .OrderByDescending(w => w.CreatedAt)
                .ToListAsync();
        }
    }
}