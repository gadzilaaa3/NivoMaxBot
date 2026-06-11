using MediatR;
using NivoMaxBot.Application.Common.Attributes;
using NivoMaxBot.Domain.Constants;

namespace NivoMaxBot.Application.Features.Orders.Commands.UpdateStatus
{
    [Authorize(RequiredRole = AdminRole.Admin)]
    public class UpdateOrderStatusCommand : IRequest<bool>
    {
        public int OrderId { get; set; }
        public string NewStatus { get; set; } = string.Empty;
    }
}
