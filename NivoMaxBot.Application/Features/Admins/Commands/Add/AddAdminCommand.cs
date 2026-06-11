using MediatR;
using NivoMaxBot.Application.Common.Attributes;

namespace NivoMaxBot.Application.Features.Admins.Commands.Add
{
    [Authorize(RequiredRole = AdminRole.SuperAdmin)]
    public class AddAdminCommand : IRequest<bool>
    {
        public long MaxId { get; set; }
        public string? UserName { get; set; }
        public bool IsSuperAdmin { get; set; }
    }
}
