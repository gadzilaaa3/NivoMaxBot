using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NivoMaxBot.Domain.Entities;

namespace NivoMaxBot.Infrastructure.Data.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders");
            builder.HasKey(o => o.Id);

            builder.Property(o => o.Status)
                .IsRequired()
                .HasMaxLength(50)
                .HasConversion<string>(); // если будет enum, замените на .HasConversion(new EnumToStringConverter<OrderStatus>())

            builder.Property(o => o.ContactPhone)
                .HasMaxLength(20);

            builder.Property(o => o.ContactEmail)
                .HasMaxLength(100);

            builder.Property(o => o.CustomerName)
                .HasMaxLength(500);

            // Связь с User
            builder.HasOne(o => o.UserNavigation)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Связь с OrderDetail
            builder.HasMany(o => o.Details)
                .WithOne(od => od.OrderNavigation)
                .HasForeignKey(od => od.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
