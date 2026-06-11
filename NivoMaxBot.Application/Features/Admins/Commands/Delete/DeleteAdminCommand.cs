using MediatR;
using NivoMaxBot.Application.Common.Attributes;

namespace NivoMaxBot.Application.Features.Admins.Commands.Delete
{
    [Authorize(RequiredRole = AdminRole.SuperAdmin)]
    public class DeleteAdminCommand : IRequest<bool>
    {
        public int AdminId { get; set; }
    }
}
