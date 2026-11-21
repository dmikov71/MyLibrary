using MyLibrary.Domain.Entities;

namespace MyLibrary.Domain.Repositories
{
    public interface IGenreRepository
    {
        Task<IEnumerable<Genre>> GetAllAsync();
        IQueryable<Genre> GetAllWithSubgenres();
        IQueryable<Genre> GetQueryable();
        IQueryable<Genre> GetSortedByNameQueryable();
        Task<Genre?> GetByIdAsync(int id);
        Task AddAsync(Genre genre);
        Task UpdateAsync(Genre genre);
        Task DeleteAsync(int id);
    }
}
