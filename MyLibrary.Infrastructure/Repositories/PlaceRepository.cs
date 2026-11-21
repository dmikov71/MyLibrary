using Microsoft.EntityFrameworkCore;
using MyLibrary.Domain.Entities;
using MyLibrary.Domain.Repositories;

namespace MyLibrary.Infrastructure.Repositories
{
    public class PlaceRepository : IPlaceRepository
    {
        private readonly MyLibraryDbContext _context;

        public PlaceRepository(MyLibraryDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Place>> GetAllAsync()
        => await _context.Places.ToListAsync();

        public IQueryable<Place> GetQueryable()
           => _context.Places;

        public IQueryable<Place> GetSortedByNameQueryable()
         => _context.Places
                .OrderBy(p => p.Name == null)   
                .ThenBy(p => p.Name)            
                .ThenBy(p => p.PlaceID);

        public async Task<Place?> GetByIdAsync(int id)
        => await _context.Places.FindAsync(id);

        public async Task AddAsync(Place place)
        {
            _context.Places.Add(place);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Place place)
        {
            _context.Places.Update(place);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var place = await _context.Places.FindAsync(id);
            if (place != null)
            {
                _context.Places.Remove(place);
                await _context.SaveChangesAsync();
            }
        }
        
    }
}
