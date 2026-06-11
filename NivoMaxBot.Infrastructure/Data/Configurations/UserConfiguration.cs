using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NivoMaxBot.Domain.Entities;

namespace NivoMaxBot.Infrastructure.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");
            builder.HasKey(u => u.Id);

            builder.Property(u => u.MaxId)
                .IsRequired();
            builder.HasIndex(u => u.MaxId)
                .IsUnique()
                .HasDatabaseName("IX_Users_MaxId");

            // Связь 1:1 с Basket (User зависит от Basket)
            builder.HasOne(u => u.BasketNavigation)
                .WithOne(b => b.UserNavigation)
                .HasForeignKey<User>(u => u.BasketId)
                .OnDelete(DeleteBehavior.Restrict);

            // Связь 1:many с Order
            builder.HasMany(u => u.Orders)
                .WithOne(o => o.UserNavigation)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Связь 1:many с WarrantyRequest
            builder.HasMany(u => u.WarrantyRequests)
                .WithOne(w => w.UserNavigation)
                .HasForeignKey(w => w.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Связь 1:many с ConsultationRequests
            builder.HasMany(u => u.ConsultationRequests)
                .WithOne(cr => cr.UserNavigation)
                .HasForeignKey(cr => cr.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
