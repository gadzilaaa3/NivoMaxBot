using MediatR;
using NivoMaxBot.Application.Common.Attributes;

namespace NivoMaxBot.Application.Features.Categories.Commands.Delete
{
    [Authorize(RequiredRole = AdminRole.SuperAdmin)]
    public class DeleteCategoryCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }
}
