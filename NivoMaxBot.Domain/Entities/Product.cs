using NivoMaxBot.Domain.Entities.Base;

namespace NivoMaxBot.Domain.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public string? PhotoMaxFileId { get; set; }

        public string? PhotoUrl { get; set; }

        public bool IsAvailable { get; set; } = true;

        public int WarrantyInMonths { get; set; }

        public ICollection<Category> Categories { get; set; } = [];

        public ICollection<BasketDetail> BasketDetails { get; set; } = [];

        public ICollection<OrderDetail> OrderDetails { get; set; } = [];
    }
}
