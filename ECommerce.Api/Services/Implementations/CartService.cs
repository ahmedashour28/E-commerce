using AutoMapper;
using ECommerce.Api.Dtos.Cart;
using ECommerce.Api.Entities;
using ECommerce.Api.Repositories.Interfaces;
using ECommerce.Api.Services.Interfaces;

namespace ECommerce.Api.Services.Implementations
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepo;
        private readonly IProductRepository _productRepo;
        private readonly IMapper _mapper;

        public CartService(ICartRepository cartRepo, IProductRepository productRepo, IMapper mapper)
        {
            _cartRepo = cartRepo;
            _productRepo = productRepo;
            _mapper = mapper;
        }

        public async Task<CartDto> AddItemToCartAsync(string buyerId, Guid productId, int quantity)
        {
            var cart = await _cartRepo.GetCartAsync(buyerId);

            if (cart == null)
            {
                cart = new Cart { BuyerId = buyerId };
                await _cartRepo.CreateCartAsync(cart);
            }

            var product = await _productRepo.GetByIdAsync(productId);
            if (product == null)
                throw new Exception("Product not found");

            cart.AddItem(product, quantity);
            await _cartRepo.SaveChangesAsync();
            return _mapper.Map<CartDto>(cart);
        }

        public async Task DeleteCartAsync(string buyerId)
        {
            await _cartRepo.DeleteCartAsync(buyerId);
        }

        public async Task<CartDto> GetCartAsync(string buyerId)
        {
            var cart = await _cartRepo.GetCartAsync(buyerId);
            return _mapper.Map<CartDto>(cart ?? new Cart { BuyerId = buyerId });
        }

        public async Task RemoveItemFromCartAsync(string buyerId, Guid productId, int quantity)
        {
            var cart = await _cartRepo.GetCartAsync(buyerId);
            if (cart == null)
                throw new Exception("Cart not found");
            cart.RemoveItem(productId, quantity);
            await _cartRepo.SaveChangesAsync();
        }
    }
}
