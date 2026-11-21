using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyLibrary.Domain.Entities;

namespace MyLibrary.Infrastructure.Configurations
{
    public class BookConfiguration : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            builder.HasKey(b => b.BookID);

            builder.Property(b => b.LibraryNumber)
                .IsRequired();

            builder.Property(b => b.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(b => b.Publishing)
                .HasMaxLength(100);

            builder.Property(b => b.Year);

            builder.Property(b => b.Pages);

            builder.Property(b => b.Volumes)
                .IsRequired();

            builder.Property(b => b.ISBN)
                .HasMaxLength(20);
            builder.Property(b => b.CoverImage)   
            .HasMaxLength(500);

            builder.Property(b => b.Read)
                .IsRequired();

            builder.Property(b => b.Type)
                .IsRequired();

            builder.Property(b => b.Subtype);

            builder.Property(b => b.Raiting);

            // Явно указываем, что CategoryId — это FK
            builder.HasOne(b => b.Category)
                   .WithMany(c => c.Books)
                   .HasForeignKey(b => b.CategoryID);

            // Явно указываем, что GenryId — это FK
            builder.HasOne(b => b.Genre)
                   .WithMany(g => g.Books)
                   .HasForeignKey(b => b.GenreID);

            // Явно указываем, что SubgenryId — это FK
            builder.HasOne(b => b.Subgenre)
                   .WithMany(s => s.Books)
                   .HasForeignKey(b => b.SubgenreID);

            // Явно указываем, что PlaceId — это FK
            builder.HasOne(b => b.Place)
                   .WithMany(p => p.Books)
                   .HasForeignKey(b => b.PlaceID);
        }
    }
}
