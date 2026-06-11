using MediatR;

namespace NivoMaxBot.Application.Features.WarrantyRequest.Commands.Create
{
    public class CreateWarrantyRequestCommand : IRequest<int>
    {
        public long UserMaxId { get; set; }

        public string? INN { get; set; }

        public string City { get; set; } = string.Empty;

        public string ContactPhone { get; set; } = string.Empty;

        public string ContactPerson { get; set; } = string.Empty;

        public string ContactEmail { get; set; } = string.Empty;

        public string ProblemDescription { get; set; } = string.Empty;

        public string ProductSerialNumber { get; set; } = string.Empty;
    }
}