using ECommerce.Api.Dtos.Review;
using ECommerce.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.Api.Controllers
{
    [ApiController]
    [Route("api/products/{productId}/reviews")]
    public class ReviewsController : BaseController
    {
        private readonly IReviewService _reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet]
        public async Task<IActionResult> GetReviews(Guid productId)
        {
            var reviews = await _reviewService.GetReviewsForProductAsync(productId);
            return Ok(reviews);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateReview(Guid productId, [FromBody] CreateReviewDto dto)
        {
            try
            {
                var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdString)) return Unauthorized();
                var userId = Guid.Parse(userIdString);

                var userName = User.Identity?.Name;
                if (string.IsNullOrEmpty(userName)) return Unauthorized();

                var review = await _reviewService.AddReviewAsync(userId, userName, productId, dto);
                return Ok(review);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}