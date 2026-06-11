using NivoMaxBot.Domain.Entities.Base;

namespace NivoMaxBot.Domain.Entities
{
    public class Category : BaseEntity
    {
        public string Name { get; set; } = string.Empty;

        public int Order { get; set; }

        public int? ParentId { get; set; }

        public Category? ParentNavigation { get; set; }

        public ICollection<Category> Children { get; set; } = [];

        public ICollection<Product> Products { get; set; } = [];
    }
}
