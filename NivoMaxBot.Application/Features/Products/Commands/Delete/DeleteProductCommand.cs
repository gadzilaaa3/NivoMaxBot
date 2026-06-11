using MediatR;
using NivoMaxBot.Application.Common.Attributes;

namespace NivoMaxBot.Application.Features.Products.Commands.Delete
{
    [Authorize(RequiredRole = AdminRole.Admin)]
    public class DeleteProductCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }
}
