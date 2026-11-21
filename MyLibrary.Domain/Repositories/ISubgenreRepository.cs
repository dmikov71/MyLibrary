using MyLibrary.Domain.Entities;

namespace MyLibrary.Domain.Repositories
{
    public interface ISubgenreRepository
    {
        Task<IEnumerable<Subgenre>> GetAllAsync();
        IQueryable<Subgenre> GetQueryable();
        IQueryable<Subgenre> GetSortedByNameQueryable();
        Task<Subgenre?> GetByIdAsync(int id);
        Task AddAsync(Subgenre subgenre);
        Task UpdateAsync(Subgenre subgenre);
        Task DeleteAsync(int id);
    }
}
