using MediatR;
using NivoMaxBot.Application.Common.Exceptions;
using NivoMaxBot.Application.Interfaces;
using NivoMaxBot.Domain.Entities;
using NivoMaxBot.Domain.Interfaces.Repositories;

namespace NivoMaxBot.Application.Features.Admins.Commands.UpdateRole
{
    public class UpdateAdminRoleCommandHandler : IRequestHandler<UpdateAdminRoleCommand, bool>
    {
        private readonly IAdminRepository _adminRepository;
        private readonly ICurrentUserService _currentUserService;

        public UpdateAdminRoleCommandHandler(
            IAdminRepository adminRepository,
            ICurrentUserService currentUserService)
        {
            _adminRepository = adminRepository;
            _currentUserService = currentUserService;
        }

        public async Task<bool> Handle(UpdateAdminRoleCommand request, CancellationToken cancellationToken)
        {
            var admin = await _adminRepository.GetByIdAsync(request.AdminId, cancellationToken);
            if (admin == null)
                throw new NotFoundException(nameof(Admin), request.AdminId);

            // Лучше запретить себе снимать суперадмина, чтобы не потерять доступ.
            var currentAdmin = await _adminRepository.GetByMaxIdAsync(_currentUserService.MaxId.Value, cancellationToken);
            if (currentAdmin?.Id == admin.Id && !request.IsSuperAdmin)
                throw new BusinessRuleViolationException("Нельзя снять роль суперадмина с самого себя.");

            admin.IsSuperAdmin = request.IsSuperAdmin;
            _adminRepository.Update(admin);
            await _adminRepository.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
