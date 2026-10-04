using ECommerce.Api.Dtos.Order;
using ECommerce.Api.Helpers;
using ECommerce.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ECommerce.Api.Controllers
{
    [Authorize]
    [ApiController]
    public class OrdersController : BaseController
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto)
        {
            try
            {
                var buyerId = User.Identity?.Name;
                if (string.IsNullOrEmpty(buyerId)) return Unauthorized();

                var order = await _orderService.CreateOrderAsync(buyerId, dto);
                return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, order);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUserOrders()
        {
            var buyerId = User.Identity?.Name;
            if (string.IsNullOrEmpty(buyerId)) return Unauthorized();

            var orders = await _orderService.GetUserOrdersAsync(buyerId);
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(Guid id)
        {
            var buyerId = User.Identity?.Name;
            if (string.IsNullOrEmpty(buyerId)) return Unauthorized();

            var order = await _orderService.GetOrderByIdAsync(id, buyerId);

            if (order == null) return NotFound();

            return Ok(order);
        }

        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllOrdersAdmin([FromQuery] OrderParams orderParams)
        {
            var orders = await _orderService.GetAllOrdersAsync(orderParams);

            var paginationMetadata = new
            {
                orders.TotalCount,
                orders.PageSize,
                orders.CurrentPage,
                orders.TotalPages,
                orders.HasNext,
                orders.HasPrevious
            };

            Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

            return Ok(orders);
        }

        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateOrderStatus(Guid id, [FromBody] string status)
        {
            await _orderService.UpdateOrderStatusAsync(id, status);
            return NoContent();
        }
    }
}