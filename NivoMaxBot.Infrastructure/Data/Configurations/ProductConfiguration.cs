using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NivoMaxBot.Domain.Entities;

namespace NivoMaxBot.Infrastructure.Data.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(p => p.Description)
                .HasMaxLength(1000);

            builder.Property(p => p.Price)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(p => p.PhotoMaxFileId);

            builder.Property(p => p.PhotoUrl);

            builder.Property(p => p.IsAvailable)
                .HasDefaultValue(true);

            builder.Property(p => p.WarrantyInMonths)
                .HasDefaultValue(0);

            // Many-to-many с Category настроена в CategoryConfiguration
        }
    }
}
