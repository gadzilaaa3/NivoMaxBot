namespace NivoMaxBot.Application.Features.Admins.Dtos
{
    public class AdminDto
    {
        public int Id { get; set; }
        public long MaxId { get; set; }
        public string? Username { get; set; }
        public bool IsSuperAdmin { get; set; }
    }
}
