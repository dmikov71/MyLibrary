using MyLibrary.Domain.Entities;
using System.Linq.Expressions;

namespace MyLibrary.Domain.Repositories
{
    public interface IBookRepository
    {
        Task <int> GetTotalBookCountAsync();
        Task<IEnumerable<Book>> GetAllWithDetailsAsync();
        Task<Book?> GetByIdAsync(int id);
        Task<Book?> GetByIdWithDetailsAsync(int id);
        Task AddAsync(Book book);
        Task UpdateAsync(Book book);
        Task RemoveAllBookAuthorsAsync(int bookId);
        Task AddBookAuthorAsync(int bookId, int authorId, int sortOrder);
        Task SaveChangesAsync();

        Task DeleteAsync(int id);
        Task<bool> AnyAsync(Expression<Func<Book, bool>> predicate);
        Task<int> GetBookOrderInCategoryAsync(int bookId);
    }

}
