using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NivoMaxBot.Domain.Entities;

namespace NivoMaxBot.Infrastructure.Data.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("Categories");
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.Order)
                .HasDefaultValue(0);

            // Индексы
            builder.HasIndex(c => c.ParentId)
                .HasDatabaseName("IX_Categories_ParentId");

            builder.HasIndex(c => new { c.ParentId, c.Name })
                .IsUnique()
                .HasDatabaseName("IX_Categories_ParentId_Name");

            // Само-ссылка (родитель-потомок)
            builder.HasOne(c => c.ParentNavigation)
                .WithMany(c => c.Children)
                .HasForeignKey(c => c.ParentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Many-to-many с Product
            builder.HasMany(c => c.Products)
                .WithMany(p => p.Categories)
                .UsingEntity(j => j.ToTable("ProductCategories"));
        }
    }
}
