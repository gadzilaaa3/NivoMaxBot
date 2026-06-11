using MediatR;
using NivoMaxBot.Application.Common.Attributes;

namespace NivoMaxBot.Application.Features.ConsultationRequests.Commands.Delete
{
    [Authorize(RequiredRole = AdminRole.Admin)]
    public class DeleteConsultationRequestCommand : IRequest<bool>
    {
        public int RequestId { get; set; }
    }
}
