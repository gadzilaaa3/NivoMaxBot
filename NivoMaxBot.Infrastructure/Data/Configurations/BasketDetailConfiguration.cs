using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NivoMaxBot.Domain.Entities;

namespace NivoMaxBot.Infrastructure.Data.Configurations
{
    public class BasketDetailConfiguration : IEntityTypeConfiguration<BasketDetail>
    {
        public void Configure(EntityTypeBuilder<BasketDetail> builder)
        {
            builder.ToTable("BasketDetails");
            builder.HasKey(bd => bd.Id);

            builder.Property(bd => bd.ProductsQuantity)
                .IsRequired()
                .HasDefaultValue(1);

            // Уникальность: один товар в корзине может быть только в одной записи
            builder.HasIndex(bd => new { bd.BasketId, bd.ProductId })
                .IsUnique()
                .HasDatabaseName("IX_BasketDetails_BasketId_ProductId");

            // Связь с Product
            builder.HasOne(bd => bd.ProductNavigation)
                .WithMany(p => p.BasketDetails)
                .HasForeignKey(bd => bd.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
