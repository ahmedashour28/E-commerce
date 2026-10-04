using ECommerce.Api.Data;
using ECommerce.Api.Entities;
using ECommerce.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Repositories.Implementations
{
    public class CartRepository : ICartRepository
    {
        private readonly ECommerceDbContext _context;

        public CartRepository(ECommerceDbContext context)
        {
            _context = context;
        }

        public async Task<Cart> CreateCartAsync(Cart cart)
        {
            await _context.Set<Cart>().AddAsync(cart);
            await _context.SaveChangesAsync();
            return cart;
        }

        public async Task<bool> DeleteCartAsync(string buyerId)
        {
            var cart = await GetCartAsync(buyerId);
            if (cart == null) return false;

            _context.Set<Cart>().Remove(cart);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Cart?> GetCartAsync(string buyerId)
        {
            return await _context.Set<Cart>()
                .Include(i => i.Items)
                .ThenInclude(p => p.Product)
                .ThenInclude(p => p.Category)
                .Include(b => b.Items)
                .ThenInclude(p => p.Product)
                .ThenInclude(i => i.Images)
                .FirstOrDefaultAsync(x => x.BuyerId == buyerId);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
