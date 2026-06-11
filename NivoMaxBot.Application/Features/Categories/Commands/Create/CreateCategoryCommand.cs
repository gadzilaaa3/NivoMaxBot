using MediatR;
using NivoMaxBot.Application.Common.Attributes;

namespace NivoMaxBot.Application.Features.Categories.Commands.Create
{
    [Authorize(RequiredRole = AdminRole.SuperAdmin)]
    public class CreateCategoryCommand : IRequest<int>
    {
        public string Name { get; set; } = string.Empty;
        public int? ParentId { get; set; }
        public int Order { get; set; }
    }
}
