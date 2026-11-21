using Microsoft.EntityFrameworkCore;
using MyLibrary.Domain.Entities;
using MyLibrary.Domain.Repositories;

namespace MyLibrary.Infrastructure.Repositories
{
    public class GenreRepository : IGenreRepository
    {
        private readonly MyLibraryDbContext _context;
        public GenreRepository(MyLibraryDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Genre>> GetAllAsync()
        => await _context.Genres.ToListAsync();

        public IQueryable<Genre> GetAllWithSubgenres()
        {
            return _context.Genres.Include(g => g.Subgenres);
        }

        public IQueryable<Genre> GetQueryable()
        => _context.Genres;

        public IQueryable<Genre> GetSortedByNameQueryable()
        => _context.Genres
                .OrderBy(g => g.Name == null)   
                .ThenBy(g => g.Name)            
                .ThenBy(g => g.GenreID);

        public async Task<Genre?> GetByIdAsync(int id)
        => await _context.Genres.FindAsync(id);

        public async Task AddAsync(Genre genre)
        {
            _context.Genres.Add(genre);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Genre genre)
        {
            _context.Genres.Update(genre);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var genre = await _context.Genres.FindAsync(id);
            if (genre != null)
            {
                _context.Genres.Remove(genre);
                await _context.SaveChangesAsync();
            }
        }

    }
}
