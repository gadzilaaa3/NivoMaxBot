using MediatR;
using NivoMaxBot.Application.Common.Attributes;

namespace NivoMaxBot.Application.Features.Orders.Commands.Delete
{
    [Authorize(RequiredRole = AdminRole.Admin)]
    public class DeleteOrderCommand : IRequest<bool>
    {
        public int OrderId { get; set; }
    }
}
