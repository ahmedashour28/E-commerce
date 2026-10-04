using ECommerce.Api.Entities;

namespace ECommerce.Api.Repositories.Interfaces
{
    public interface IReviewRepository
    {
        Task AddAsync(Review review);
        Task<IEnumerable<Review>> GetByProductIdAsync(Guid productId);
        Task<Review?> GetByUserAndProductAsync(Guid userId, Guid productId);
    }
}