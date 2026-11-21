using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyLibrary.Domain.Entities;

namespace MyLibrary.Infrastructure.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(c => c.CategoryID);

            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.Code)
                .IsRequired()
                .HasMaxLength(3);

            // Настраиваем связь: у одного Category много Book
            builder.HasMany(c => c.Books)
                   .WithOne(c => c.Category)
                   .HasForeignKey(b => b.CategoryID)
                   .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
