using MediatR;
using NivoMaxBot.Application.Common.Attributes;

namespace NivoMaxBot.Application.Features.Admins.Commands.UpdateRole
{
    [Authorize(RequiredRole = AdminRole.SuperAdmin)]
    public class UpdateAdminRoleCommand : IRequest<bool>
    {
        public int AdminId { get; set; }
        public bool IsSuperAdmin { get; set; }
    }
}
