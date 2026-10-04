using AutoMapper;
using ECommerce.Api.Dtos.Category;
using ECommerce.Api.Entities;
using ECommerce.Api.Repositories.Interfaces;
using ECommerce.Api.Services.Interfaces;

namespace ECommerce.Api.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repo;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }
        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto dto)
        {
            var category = _mapper.Map<Category>(dto);
            await _repo.AddAsync(category);
            var createdCategory = await _repo.GetByIdAsync(category.Id);
            return _mapper.Map<CategoryDto>(createdCategory);
        }

        public async Task DeleteCategoryAsync(Guid id)
        {
            var category = await _repo.GetByIdAsync(id);
            if (category != null)
                await _repo.DeleteAsync(id);
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await _repo.GetAllAsync();
            return _mapper.Map<IEnumerable<CategoryDto>>(categories);
        }

        public async Task<CategoryDto> GetCategoryByIdAsync(Guid id)
        {
            var category = await _repo.GetByIdAsync(id);
            return _mapper.Map<CategoryDto>(category);
        }

        public async Task UpdateCategoryAsync(Guid id, CreateCategoryDto dto)
        {
            var category = await _repo.GetByIdAsync(id);
            if (category == null) return;
            _mapper.Map(dto, category);
            await _repo.UpdateAsync(category);
        }
    }
}
