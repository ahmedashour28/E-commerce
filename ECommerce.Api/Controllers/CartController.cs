using ECommerce.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers
{
    [ApiController]
    public class CartController : BaseController
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        private string GetBuyerId()
        {
            if (User.Identity?.IsAuthenticated == true)
                return User.Identity.Name!;

            if (Request.Query.TryGetValue("buyerId", out var buyerId))
                return buyerId.ToString();

            return "anonymous";
        }

        [HttpGet]
        public async Task<IActionResult> Getcart()
        {
            var cart = await _cartService.GetCartAsync(GetBuyerId());
            return Ok(cart);
        }

        [HttpPost]
        public async Task<IActionResult> AddItemTocart(Guid productId, int quantity)
        {
            try
            {
                var cart = await _cartService.AddItemToCartAsync(GetBuyerId(), productId, quantity);
                return Ok(cart);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> RemoveItemFromcart(Guid productId, int quantity = 1)
        {
            await _cartService.RemoveItemFromCartAsync(GetBuyerId(), productId, quantity);
            return Ok();
        }

        [HttpDelete("clear")]
        public async Task<IActionResult> Clearcart()
        {
            await _cartService.DeleteCartAsync(GetBuyerId());
            return NoContent();
        }
    }
}
