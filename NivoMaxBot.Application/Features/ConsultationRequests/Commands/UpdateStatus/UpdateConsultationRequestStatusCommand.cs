using MediatR;
using NivoMaxBot.Application.Common.Attributes;

namespace NivoMaxBot.Application.Features.ConsultationRequests.Commands.UpdateStatus
{
    [Authorize(RequiredRole = AdminRole.Admin)]
    public class UpdateConsultationRequestStatusCommand : IRequest<bool>
    {
        public int RequestId { get; set; }
        public string NewStatus { get; set; } = string.Empty;
    }
}
