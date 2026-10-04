using ECommerce.Api.Data;
using ECommerce.Api.Entities;
using ECommerce.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Repositories.Implementations
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly ECommerceDbContext _context;

        public ReviewRepository(ECommerceDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Review review)
        {
            await _context.Reviews.AddAsync(review);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Review>> GetByProductIdAsync(Guid productId)
        {
            return await _context.Reviews
                .Include(r => r.User)
                .Where(r => r.ProductId == productId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<Review?> GetByUserAndProductAsync(Guid userId, Guid productId)
        {
            return await _context.Reviews
                .FirstOrDefaultAsync(r => r.UserId == userId && r.ProductId == productId);
        }
    }
}