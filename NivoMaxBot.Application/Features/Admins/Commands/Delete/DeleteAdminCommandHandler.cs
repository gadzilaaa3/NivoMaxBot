using MediatR;
using NivoMaxBot.Application.Common.Exceptions;
using NivoMaxBot.Application.Interfaces;
using NivoMaxBot.Domain.Entities;
using NivoMaxBot.Domain.Interfaces.Repositories;

namespace NivoMaxBot.Application.Features.Admins.Commands.Delete
{
    public class DeleteAdminCommandHandler : IRequestHandler<DeleteAdminCommand, bool>
    {
        private readonly IAdminRepository _adminRepository;
        private readonly ICurrentUserService _currentUser;

        public DeleteAdminCommandHandler(
            IAdminRepository adminRepository, 
            ICurrentUserService currentUser)
        {
            _adminRepository = adminRepository;
            _currentUser = currentUser;
        }

        public async Task<bool> Handle(DeleteAdminCommand request, CancellationToken cancellationToken)
        {
            var admin = await _adminRepository.GetByIdAsync(request.AdminId, cancellationToken);
            if (admin == null)
                throw new NotFoundException(nameof(Admin), request.AdminId);

            // Нельзя удалить самого себя
            var currentAdmin = await _adminRepository.GetByMaxIdAsync(_currentUser.MaxId.Value, cancellationToken);
            if (currentAdmin?.Id == admin.Id)
                throw new BusinessRuleViolationException("Нельзя удалить самого себя.");

            _adminRepository.Remove(admin);
            await _adminRepository.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
