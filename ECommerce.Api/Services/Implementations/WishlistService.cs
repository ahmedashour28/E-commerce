using AutoMapper;
using ECommerce.Api.Dtos.Wishlist;
using ECommerce.Api.Entities;
using ECommerce.Api.Repositories.Interfaces;
using ECommerce.Api.Services.Interfaces;

namespace ECommerce.Api.Services.Implementations
{
    public class WishlistService : IWishlistService
    {
        private readonly IWishlistRepository _repo;
        private readonly IProductRepository _productRepo;
        private readonly IMapper _mapper;

        public WishlistService(IWishlistRepository repo, IProductRepository productRepo, IMapper mapper)
        {
            _repo = repo;
            _productRepo = productRepo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<WishlistDto>> GetWishlistAsync(Guid userId)
        {
            var items = await _repo.GetByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<WishlistDto>>(items);
        }

        public async Task<string> ToggleWishlistAsync(Guid userId, Guid productId)
        {
            var product = await _productRepo.GetByIdAsync(productId);
            if (product == null) throw new Exception("Product not found");

            var existingItem = await _repo.GetByUserAndProductAsync(userId, productId);

            if (existingItem != null)
            {
                await _repo.DeleteAsync(existingItem);
                return "Removed from wishlist";
            }
            else
            {
                var newItem = new Wishlist
                {
                    UserId = userId,
                    ProductId = productId
                };
                await _repo.AddAsync(newItem);
                return "Added to wishlist";
            }
        }
    }
}