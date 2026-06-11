using NivoMaxBot.Domain.Entities.Base;

namespace NivoMaxBot.Domain.Entities
{
    public class Admin : BaseEntity
    {
        public long MaxId { get; set; }
        public string? Username { get; set; }
        public bool IsSuperAdmin { get; set; }
    }
}
