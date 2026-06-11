using NivoMaxBot.Domain.Entities.Base;

namespace NivoMaxBot.Domain.Entities
{
    public class Basket : BaseEntity
    {
        public User? UserNavigation { get; set; }

        public ICollection<BasketDetail> Details { get; set; } = [];
    }
}
