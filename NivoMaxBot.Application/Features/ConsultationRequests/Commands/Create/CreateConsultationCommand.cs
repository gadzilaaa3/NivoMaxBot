using MediatR;

namespace NivoMaxBot.Application.Features.ConsultationRequests.Commands.Create
{
    public class CreateConsultationCommand : IRequest<int>
    {
        public long UserMaxId { get; set; }

        public string ContactName { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
    }
}
