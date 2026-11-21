using MyLibrary.Domain.Entities;
using MyLibrary.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MyLibrary.Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly MyLibraryDbContext _context;

        public CategoryRepository(MyLibraryDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
       => await _context.Categories.ToListAsync();

        public IQueryable<Category> GetQueryable()
        => _context.Categories;

        public IQueryable<Category> GetSortedByNameQueryable()
        => _context.Categories
                .OrderBy(c => c.Name == null)   
                .ThenBy(c => c.Name)            
                .ThenBy(c => c.CategoryID);

        public async Task<Category?> GetByIdAsync(int id)
        => await _context.Categories.FindAsync(id);

        public async Task AddAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
        }
       
    }
}
