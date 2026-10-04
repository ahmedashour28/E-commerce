using ECommerce.Api.Dtos.Product;
using ECommerce.Api.Helpers;
using ECommerce.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ECommerce.Api.Controllers
{
    [ApiController]
    public class ProductsController : BaseController
    {
        private readonly IProductService _service;

        public ProductsController(IProductService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts([FromQuery] ProductParams productParams)
        {
            var products = await _service.GetAllProductsAsync(productParams);

            var metadata = new
            {
                products.TotalCount,
                products.PageSize,
                products.CurrentPage,
                products.TotalPages
            };

            Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(metadata));

            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(Guid id)
        {
            var product = await _service.GetProductByIdAsync(id);
            if (product == null) return NotFound();
            return Ok(product);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateProduct([FromForm] CreateProductDto productDto)
        {
            var createdProduct = await _service.CreateProductAsync(productDto);
            return CreatedAtAction(nameof(GetProductById), new { id = createdProduct.Id }, createdProduct);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateProduct(Guid id, [FromForm] CreateProductDto productDto)
        {
            var existingProduct = await _service.GetProductByIdAsync(id);
            if (existingProduct == null) return NotFound();
            await _service.UpdateProductAsync(id, productDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            var existingProduct = await _service.GetProductByIdAsync(id);
            if (existingProduct == null) return NotFound();
            await _service.DeleteProductAsync(id);
            return NoContent();
        }
    }
}
