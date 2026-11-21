using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyLibrary.Domain.Entities;

namespace MyLibrary.Infrastructure.Configurations
{
    public class SubgenreConfiguration : IEntityTypeConfiguration<Subgenre>
    {
        public void Configure(EntityTypeBuilder<Subgenre> builder)
        {
            builder.HasKey(s => s.SubgenreID);
            builder.Property(s => s.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            // Явно указываем, что GenreId — это FK
            builder.HasOne(s => s.Genre)
                   .WithMany(g => g.Subgenres)
                   .HasForeignKey(s => s.GenreID);

            // Настраиваем связь: у одного Subgenre много Book
            builder.HasMany(s => s.Books)
                   .WithOne(b => b.Subgenre)
                   .HasForeignKey(b => b.SubgenreID)
                   .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
