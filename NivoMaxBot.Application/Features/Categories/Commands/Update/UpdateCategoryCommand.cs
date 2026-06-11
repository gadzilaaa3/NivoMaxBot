using MediatR;
using NivoMaxBot.Application.Common.Attributes;

namespace NivoMaxBot.Application.Features.Categories.Commands.Update
{
    [Authorize(RequiredRole = AdminRole.SuperAdmin)]
    public class UpdateCategoryCommand : IRequest<bool>
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int? ParentId { get; set; }
        public int Order { get; set; }
    }
}
