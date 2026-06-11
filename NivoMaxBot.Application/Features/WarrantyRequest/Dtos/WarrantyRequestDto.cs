namespace NivoMaxBot.Application.Features.WarrantyRequest.Dtos
{
    public class WarrantyRequestDto
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string? INN { get; set; }

        public string City { get; set; } = string.Empty;

        public string ContactPhone { get; set; } = string.Empty;

        public string ContactPerson { get; set; } = string.Empty;

        public string ContactEmail { get; set; } = string.Empty;

        public string ProblemDescription { get; set; } = string.Empty;

        public string ProductSerialNumber { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}
