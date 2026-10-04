using AutoMapper;
using ECommerce.Api.Dtos.Review;
using ECommerce.Api.Entities;
using ECommerce.Api.Repositories.Interfaces;
using ECommerce.Api.Services.Interfaces;

namespace ECommerce.Api.Services.Implementations
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepo;
        private readonly IProductRepository _productRepo;
        private readonly IOrderRepository _orderRepo;
        private readonly IMapper _mapper;

        public ReviewService(IReviewRepository reviewRepo, IProductRepository productRepo, IOrderRepository orderRepo, IMapper mapper)
        {
            _reviewRepo = reviewRepo;
            _productRepo = productRepo;
            _orderRepo = orderRepo;
            _mapper = mapper;
        }

        public async Task<ReviewDto> AddReviewAsync(Guid userId, string userName, Guid productId, CreateReviewDto dto)
        {
            var product = await _productRepo.GetByIdAsync(productId);
            if (product == null) throw new Exception("Product not found");

            var hasPurchased = await _orderRepo.HasUserPurchasedProductAsync(userName, productId);
            if (!hasPurchased)
                throw new Exception("You can only review products that you have purchased and received.");

            var existingReview = await _reviewRepo.GetByUserAndProductAsync(userId, productId);
            if (existingReview != null) throw new Exception("You have already reviewed this product");

            var review = _mapper.Map<Review>(dto);
            review.UserId = userId;
            review.ProductId = productId;

            await _reviewRepo.AddAsync(review);

            return _mapper.Map<ReviewDto>(review);
        }

        public async Task<IEnumerable<ReviewDto>> GetReviewsForProductAsync(Guid productId)
        {
            var reviews = await _reviewRepo.GetByProductIdAsync(productId);
            return _mapper.Map<IEnumerable<ReviewDto>>(reviews);
        }
    }
}