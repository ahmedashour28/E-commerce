using ECommerce.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.Api.Controllers
{
    [Authorize]
    [ApiController]
    public class WishlistController : BaseController
    {
        private readonly IWishlistService _service;

        public WishlistController(IWishlistService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetWishlist()
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var items = await _service.GetWishlistAsync(userId);
            return Ok(items);
        }

        [HttpPost("{productId}")]
        public async Task<IActionResult> ToggleWishlist(Guid productId)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var result = await _service.ToggleWishlistAsync(userId, productId);
                return Ok(new { message = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}