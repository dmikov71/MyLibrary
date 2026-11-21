using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyLibrary.Domain.Entities;

namespace MyLibrary.Infrastructure.Configurations
{
    public class BookAuthorConfiguration : IEntityTypeConfiguration<BookAuthor>
    {
        public void Configure(EntityTypeBuilder<BookAuthor> builder)
        {
            // Составной первичный ключ: одна пара книга-автор встречается один раз
            builder.HasKey(ba => new { ba.BookID, ba.AuthorID });

            builder.HasOne(ba => ba.Book)
                   .WithMany(b => b.BookAuthors)
                   .HasForeignKey(ba => ba.BookID)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(ba => ba.Author)
                   .WithMany(a => a.BookAuthors)
                   .HasForeignKey(ba => ba.AuthorID)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.Property(ba => ba.SortOrder)
                   .IsRequired();
        }
    }
}

