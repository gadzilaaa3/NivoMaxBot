namespace NivoMaxBot.Application.Features.ConsultationRequests.Dtos
{
    public class ConsultationRequestDto
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string CustomerName { get; set; } = string.Empty; // ContactName

        public string City { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}
