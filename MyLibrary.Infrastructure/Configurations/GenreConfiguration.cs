using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyLibrary.Domain.Entities;

namespace MyLibrary.Infrastructure.Configurations
{
    public class GenreConfiguration : IEntityTypeConfiguration<Genre>
    {
        public void Configure(EntityTypeBuilder<Genre> builder)
        {
            builder.HasKey(g => g.GenreID);
            builder.Property(g => g.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            // Настраиваем связь: у одного Genre много Subgenre
            builder.HasMany(g => g.Subgenres)
                   .WithOne(s => s.Genre)
                   .HasForeignKey(s => s.GenreID)
                   .OnDelete(DeleteBehavior.Cascade);

            // Настраиваем связь: у одного Genre много Book
            builder.HasMany(g => g.Books)
                   .WithOne(b => b.Genre)
                   .HasForeignKey(b => b.GenreID)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(g => g.Category)
                    .WithMany(c => c.Genres) 
                    .HasForeignKey(g => g.CategoryID)
                    .OnDelete(DeleteBehavior.NoAction); 
        
    }
    }
}
