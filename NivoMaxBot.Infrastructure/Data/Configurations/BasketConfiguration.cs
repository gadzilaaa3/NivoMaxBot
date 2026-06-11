using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NivoMaxBot.Domain.Entities;

namespace NivoMaxBot.Infrastructure.Data.Configurations
{
    public class BasketConfiguration : IEntityTypeConfiguration<Basket>
    {
        public void Configure(EntityTypeBuilder<Basket> builder)
        {
            builder.ToTable("Baskets");
            builder.HasKey(b => b.Id);

            // Связь 1:many с BasketDetail
            builder.HasMany(b => b.Details)
                .WithOne(bd => bd.BasketNavigation)
                .HasForeignKey(bd => bd.BasketId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
