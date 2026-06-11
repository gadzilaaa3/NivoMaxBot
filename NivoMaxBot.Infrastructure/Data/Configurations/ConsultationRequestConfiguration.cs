using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NivoMaxBot.Domain.Entities;

namespace NivoMaxBot.Infrastructure.Data.Configurations
{
    public class ConsultationRequestConfiguration : IEntityTypeConfiguration<ConsultationRequest>
    {
        public void Configure(EntityTypeBuilder<ConsultationRequest> builder)
        {
            builder.ToTable("ConsultationRequests");
            builder.HasKey(cr => cr.Id);

            builder.Property(cr => cr.Status)
                .IsRequired()
                .HasMaxLength(50)
                .HasConversion<string>();

            builder.HasIndex(cr => cr.Status)
                .HasDatabaseName("IX_ConsultationRequests_Status");

            // Связь с пользователем (один ко многим)
            builder.HasOne(cr => cr.UserNavigation)
                .WithMany(u => u.ConsultationRequests)
                .HasForeignKey(cr => cr.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
