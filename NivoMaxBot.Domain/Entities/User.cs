using NivoMaxBot.Domain.Entities.Base;

namespace NivoMaxBot.Domain.Entities
{
    public class User : BaseEntity
    {
        public long MaxId { get; set; }

        public int BasketId { get; set; }

        public Basket? BasketNavigation { get; set; }

        public ICollection<WarrantyRequest> WarrantyRequests { get; set; } = [];
        
        public ICollection<Order> Orders { get; set; } = [];

        public ICollection<ConsultationRequest> ConsultationRequests { get; set; } = [];
    }
}
