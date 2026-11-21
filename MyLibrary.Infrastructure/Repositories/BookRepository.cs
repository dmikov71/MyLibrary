using Microsoft.EntityFrameworkCore;
using MyLibrary.Domain.Entities;
using MyLibrary.Domain.Repositories;
using System.Linq.Expressions;

namespace MyLibrary.Infrastructure.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly MyLibraryDbContext _context;
        public BookRepository(MyLibraryDbContext context) => _context = context;

        public async Task<IEnumerable<Book>> GetAllWithDetailsAsync()
        {
            return await _context.Books
                .Include(b => b.BookAuthors)
                    .ThenInclude(ba => ba.Author)
                .Include(b => b.Category)
                .Include(b => b.Genre)
                .Include(b => b.Subgenre)
                .Include(b => b.Place)
                .ToListAsync();
        }

        public async Task<Book?> GetByIdAsync(int id)
         => await _context.Books.FindAsync(id);

        public async Task<Book?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Books
                .Include(b => b.BookAuthors)
                    .ThenInclude(ba => ba.Author)
                .Include(b => b.Genre)
                .Include(b => b.Category)
                .Include(b => b.Subgenre)
                .Include(b => b.Place)
                .FirstOrDefaultAsync(b => b.BookID == id);
        }

        public async Task AddAsync(Book book)
        {
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Book book)
        {
            _context.Books.Update(book);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAllBookAuthorsAsync(int bookId)
        {
            var existing = await _context.BookAuthors
                .Where(ba => ba.BookID == bookId)
                .ToListAsync();
            _context.BookAuthors.RemoveRange(existing);
        }

        public async Task AddBookAuthorAsync(int bookId, int authorId, int sortOrder)
        {
            _context.BookAuthors.Add(new BookAuthor
            {
                BookID = bookId,
                AuthorID = authorId,
                SortOrder = sortOrder
            });
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Удаляем связи «книга-авторы»
                var bookAuthors = await _context.BookAuthors
                    .Where(ba => ba.BookID == id)
                    .ToListAsync();

                if (bookAuthors.Any())
                {
                    _context.BookAuthors.RemoveRange(bookAuthors);
                }

                // 2. Удаляем саму книгу
                var book = await _context.Books.FindAsync(id);
                if (book != null)
                {
                    _context.Books.Remove(book);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<int> GetTotalBookCountAsync()
        {
            return await _context.Books.CountAsync();
        }

        public Task<bool> AnyAsync(Expression<Func<Book, bool>> predicate)
    => _context.Books.AnyAsync(predicate);

        public async Task<int> GetBookOrderInCategoryAsync(int bookId)
        {
            var book = await _context.Books
        .Where(b => b.BookID == bookId)
        .Select(b => new { b.BookID, b.CategoryID })
        .FirstOrDefaultAsync();

            if (book == null || book.CategoryID == null)
                return 0;

            var count = await _context.Books
                .Where(b => b.CategoryID == book.CategoryID && b.LibraryNumber <= book.BookID)
                .CountAsync();

            return count;
        }
    }
}
