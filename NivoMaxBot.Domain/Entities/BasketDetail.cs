using NivoMaxBot.Domain.Entities.Base;

namespace NivoMaxBot.Domain.Entities
{
    public class BasketDetail : BaseEntity
    {
        public int BasketId { get; set; }

        public Basket? BasketNavigation { get; set; }

        public int ProductsQuantity { get; set; }

        public int ProductId { get; set; }

        public Product? ProductNavigation { get; set; }
    }
}
