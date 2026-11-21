using MyLibrary.Domain.Entities;
using MyLibrary.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MyLibrary.Infrastructure.Repositories
{
    public class AuthorRepository : IAuthorRepository
    {
        private readonly MyLibraryDbContext _context;

        public AuthorRepository(MyLibraryDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Author>> GetAllAsync()
            => await _context.Authors
                .OrderBy(a => a.LastName)      
                .ThenBy(a => a.FirstName)
                .ToListAsync();
        public async Task<bool> HasBooksAsync(int authorId)
    => await _context.BookAuthors.AnyAsync(ba => ba.AuthorID == authorId);


        public IQueryable<Author> GetSortedByNameQueryable()
            => _context.Authors
                .OrderBy(a => a.LastName)       // фамилия — главный ключ
                .ThenBy(a => a.FirstName)       // при одинаковых фамилиях — по имени
                .ThenBy(a => a.AuthorID);       // для стабильного порядка

        public async Task<Author?> GetByIdAsync(int id)
            => await _context.Authors.FindAsync(id);

        public async Task AddAsync(Author author)
        {
            _context.Authors.Add(author);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Author author)
        {
            _context.Authors.Update(author);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var author = await _context.Authors.FindAsync(id);
            if (author != null)
            {
                _context.Authors.Remove(author);
                await _context.SaveChangesAsync();
            }
        }
    }
}
