using NivoMaxBot.Domain.Entities.Base;

namespace NivoMaxBot.Domain.Entities
{
    public class Order : BaseEntity
    {
        public int UserId { get; set; }

        public User? UserNavigation { get; set; }

        public string Status { get; set; } = string.Empty;

        public ICollection<OrderDetail> Details { get; set; } = [];

        public string ContactPhone { get; set; } = string.Empty;

        public string CustomerName { get; set; } = string.Empty;

        public string? ContactEmail { get; set; }

        public string? INN { get; set; }
    }
}
