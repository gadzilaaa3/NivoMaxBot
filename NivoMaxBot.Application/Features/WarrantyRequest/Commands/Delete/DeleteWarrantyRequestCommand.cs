using MediatR;
using NivoMaxBot.Application.Common.Attributes;

namespace NivoMaxBot.Application.Features.WarrantyRequest.Commands.Delete
{
    [Authorize(RequiredRole = AdminRole.Admin)]
    public class DeleteWarrantyRequestCommand : IRequest<bool>
    {
        public int RequestId { get; set; }
    }
}
