using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyLibrary.Domain.Entities;

namespace MyLibrary.Infrastructure.Configurations
{
    public class PlaceConfiguration : IEntityTypeConfiguration<Place>
    {
        public void Configure(EntityTypeBuilder<Place> builder)
        {
            builder.HasKey(p => p.PlaceID);

            builder.Property(p => p.Name)
                .HasMaxLength(100);

            // Настраиваем связь: у одного Place много Book
            builder.HasMany(p => p.Books)
                   .WithOne(b => b.Place)
                   .HasForeignKey(b => b.PlaceID)
                   .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
