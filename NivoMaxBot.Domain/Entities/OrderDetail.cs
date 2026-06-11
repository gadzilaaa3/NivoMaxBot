using NivoMaxBot.Domain.Entities.Base;

namespace NivoMaxBot.Domain.Entities
{
    public class OrderDetail : BaseEntity
    {
        public int OrderId { get; set; }

        public Order? OrderNavigation { get; set; }

        public int ProductsQuantity { get; set; }

        public int ProductId { get; set; }

        public Product? ProductNavigation { get; set; }

        public decimal PriceAtOrder { get; set; }
    }
}
