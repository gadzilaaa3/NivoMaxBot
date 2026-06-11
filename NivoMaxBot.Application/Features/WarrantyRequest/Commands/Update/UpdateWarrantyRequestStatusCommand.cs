using MediatR;
using NivoMaxBot.Application.Common.Attributes;

namespace NivoMaxBot.Application.Features.WarrantyRequest.Commands.Update
{
    [Authorize(RequiredRole = AdminRole.Admin)]
    public class UpdateWarrantyRequestStatusCommand : IRequest<bool>
    {
        public int RequestId { get; set; }
        public string NewStatus { get; set; } = string.Empty;
    }
}
