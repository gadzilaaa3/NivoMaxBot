using NivoMaxBot.Domain.Entities.Base;

namespace NivoMaxBot.Domain.Entities
{
    public class ConsultationRequest : BaseEntity
    {
        public int UserId { get; set; }

        public User? UserNavigation { get; set; }

        public string ContactName { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty; 

        public string Description { get; set; } = string.Empty;
    }
}
