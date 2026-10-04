using ECommerce.Api.Dtos.Product;
using ECommerce.Api.Helpers;

namespace ECommerce.Api.Services.Interfaces
{
    public interface IProductService
    {
        Task<PagedList<ProductDto>> GetAllProductsAsync(ProductParams productParams);
        Task<ProductDto?> GetProductByIdAsync(Guid id);
        Task<ProductDto> CreateProductAsync(CreateProductDto newProduct);
        Task UpdateProductAsync(Guid id, CreateProductDto updatedProduct);
        Task DeleteProductAsync(Guid id);
    }
}
