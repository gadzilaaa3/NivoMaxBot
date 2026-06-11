using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NivoMaxBot.Domain.Entities;

namespace NivoMaxBot.Infrastructure.Data.Configurations
{
    public class WarrantyRequestConfiguration : IEntityTypeConfiguration<WarrantyRequest>
    {
        public void Configure(EntityTypeBuilder<WarrantyRequest> builder)
        {
            builder.ToTable("WarrantyRequests");
            builder.HasKey(w => w.Id);

            builder.Property(w => w.INN).HasMaxLength(20);
            builder.Property(w => w.City).HasMaxLength(100);
            builder.Property(w => w.ContactPhone).HasMaxLength(20);
            builder.Property(w => w.ContactPerson).HasMaxLength(200);
            builder.Property(w => w.ContactEmail).HasMaxLength(100);
            builder.Property(w => w.ProblemDescription).HasMaxLength(2000);
            builder.Property(w => w.ProductSerialNumber).HasMaxLength(100);

            builder.Property(w => w.Status)
                .IsRequired()
                .HasMaxLength(50)
                .HasConversion<string>();

            // Связь с User
            builder.HasOne(w => w.UserNavigation)
                .WithMany(u => u.WarrantyRequests)
                .HasForeignKey(w => w.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
