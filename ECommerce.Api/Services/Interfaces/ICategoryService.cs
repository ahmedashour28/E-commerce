using ECommerce.Api.Dtos.Category;

namespace ECommerce.Api.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
        Task<CategoryDto> GetCategoryByIdAsync(Guid id);
        Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto dto);
        Task UpdateCategoryAsync(Guid id, CreateCategoryDto dto);
        Task DeleteCategoryAsync(Guid id);
    }
}
