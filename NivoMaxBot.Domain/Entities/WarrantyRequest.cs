using NivoMaxBot.Domain.Entities.Base;

namespace NivoMaxBot.Domain.Entities
{
    public class WarrantyRequest : BaseEntity
    {
        public int UserId { get; set; }

        public User? UserNavigation { get; set; }

        public string Status { get; set; } = string.Empty;

        public string ContactPerson { get; set; } = string.Empty;

        public string ProductSerialNumber { get; set; } = string.Empty;

        public string ProblemDescription { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public string ContactPhone { get; set; } = string.Empty;

        public string ContactEmail { get; set; } = string.Empty;

        public string? INN { get; set; }
    }
}
