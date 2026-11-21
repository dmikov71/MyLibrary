using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyLibrary.Domain.Entities;
using MyLibrary.Infrastructure.Configurations;
using MyLibrary.Infrastructure.Models;
namespace MyLibrary.Infrastructure
{
    public class MyLibraryDbContext : IdentityDbContext<ApplicationUser>
    {
        public MyLibraryDbContext(DbContextOptions<MyLibraryDbContext> options)
        : base(options) { }

        public DbSet<Book> Books => Set<Book>();
        public DbSet<Author> Authors => Set<Author>();
        public DbSet<BookAuthor> BookAuthors { get; set; }
        public DbSet<Genre> Genres => Set<Genre>();
        public DbSet<Subgenre> Subgenres => Set<Subgenre>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Place> Places => Set<Place>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Fluent API: ключи, связи, индексы, конвертеры и т. п.
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new AuthorConfiguration());
            modelBuilder.ApplyConfiguration(new BookAuthorConfiguration());
            modelBuilder.ApplyConfiguration(new BookConfiguration());
            modelBuilder.ApplyConfiguration(new CategoryConfiguration());
            modelBuilder.ApplyConfiguration(new GenreConfiguration());
            modelBuilder.ApplyConfiguration(new PlaceConfiguration());
            modelBuilder.ApplyConfiguration(new SubgenreConfiguration());
        }
    }
}
