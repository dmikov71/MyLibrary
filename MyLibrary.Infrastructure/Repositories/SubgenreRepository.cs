using Microsoft.EntityFrameworkCore;
using MyLibrary.Domain.Entities;
using MyLibrary.Domain.Repositories;

namespace MyLibrary.Infrastructure.Repositories
{
    public class SubgenreRepository : ISubgenreRepository
    {
        public readonly MyLibraryDbContext _context;
        public SubgenreRepository(MyLibraryDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Subgenre>> GetAllAsync()
        => await _context.Subgenres.ToListAsync();

        public IQueryable<Subgenre> GetQueryable()
        => _context.Subgenres;

        public IQueryable<Subgenre> GetSortedByNameQueryable()
        => _context.Subgenres
                .OrderBy(s => s.Name == null)
                .ThenBy(s => s.Name)
                .ThenBy(s => s.GenreID);

        public async Task<Subgenre?> GetByIdAsync(int id)
        => await _context.Subgenres.FindAsync(id);

        public async Task AddAsync(Subgenre subgenre)
        {
            _context.Subgenres.Add(subgenre);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Subgenre subgenre)
        {
            _context.Subgenres.Update(subgenre);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var subgenre = await _context.Subgenres.FindAsync(id);
            if(subgenre != null)
            {
                _context.Subgenres.Remove(subgenre);
                await _context.SaveChangesAsync();
            }
        }
    }
}
