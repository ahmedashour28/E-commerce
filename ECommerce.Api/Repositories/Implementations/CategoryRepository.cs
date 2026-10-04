using ECommerce.Api.Data;
using ECommerce.Api.Entities;
using ECommerce.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Repositories.Implementations
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ECommerceDbContext _context;

        public CategoryRepository(ECommerceDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _context.Categories.Include(c => c.ParentCategory).ToListAsync();
        }

        public async Task<Category?> GetByIdAsync(Guid id)
        {
            return await _context.Categories.Include(c => c.ParentCategory).FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task AddAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }
    }
}