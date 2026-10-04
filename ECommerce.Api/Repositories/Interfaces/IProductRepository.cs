using ECommerce.Api.Entities;
using ECommerce.Api.Helpers;

namespace ECommerce.Api.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task<PagedList<Product>> GetAllAsync(ProductParams productParams);
        Task<Product?> GetByIdAsync(Guid id);
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(Guid id);
    }
}
