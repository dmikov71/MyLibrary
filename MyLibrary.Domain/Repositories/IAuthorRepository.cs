using MyLibrary.Domain.Entities;

namespace MyLibrary.Domain.Repositories
{
    public interface IAuthorRepository
    {
        Task<IEnumerable<Author>> GetAllAsync();
        Task<bool> HasBooksAsync(int authorId);
        Task<Author?> GetByIdAsync(int id);
        Task AddAsync(Author author);
        Task UpdateAsync(Author author);
        Task DeleteAsync(int id);

    
        IQueryable<Author> GetSortedByNameQueryable();
    }
}
