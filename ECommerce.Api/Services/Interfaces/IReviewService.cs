using ECommerce.Api.Dtos.Review;

namespace ECommerce.Api.Services.Interfaces
{
    public interface IReviewService
    {
        Task<ReviewDto> AddReviewAsync(Guid userId, string userName, Guid productId, CreateReviewDto dto);
        Task<IEnumerable<ReviewDto>> GetReviewsForProductAsync(Guid productId);
    }
}
