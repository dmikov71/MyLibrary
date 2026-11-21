using MyLibrary.Domain.Entities;

namespace MyLibrary.Domain.Repositories
{
    public interface IPlaceRepository
    {
        Task<IEnumerable<Place>> GetAllAsync();
        Task<Place?> GetByIdAsync(int id);
        Task AddAsync(Place place);
        Task UpdateAsync(Place place);
        Task DeleteAsync(int id);

        IQueryable<Place> GetQueryable();
        IQueryable<Place> GetSortedByNameQueryable();
    }
}
