using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyLibrary.Domain.Entities;

namespace MyLibrary.Infrastructure.Configurations
{
    public class AuthorConfiguration : IEntityTypeConfiguration<Author>
    {
        public void Configure(EntityTypeBuilder<Author> builder)
        {
            builder.HasKey(a => a.AuthorID);
            builder.Property(a => a.LastName)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(a => a.FirstName)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(a => a.MiddleName)
                   .HasMaxLength(50);

            // Настраиваем связь
            builder.HasMany(a => a.BookAuthors)
                    .WithOne(ba => ba.Author)
                    .HasForeignKey(ba => ba.AuthorID)
                    .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
