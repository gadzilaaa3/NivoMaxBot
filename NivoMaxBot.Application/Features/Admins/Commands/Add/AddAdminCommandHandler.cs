using MediatR;
using NivoMaxBot.Application.Common.Exceptions;
using NivoMaxBot.Domain.Entities;
using NivoMaxBot.Domain.Interfaces.Repositories;

namespace NivoMaxBot.Application.Features.Admins.Commands.Add
{
    public class AddAdminCommandHandler : IRequestHandler<AddAdminCommand, bool>
    {
        private readonly IAdminRepository _adminRepository;

        public AddAdminCommandHandler(IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }

        public async Task<bool> Handle(AddAdminCommand request, CancellationToken cancellationToken)
        {
            var existing = await _adminRepository.GetByMaxIdAsync(request.MaxId, cancellationToken);
            if (existing != null)
                throw new BusinessRuleViolationException($"Администратор с MaxId {request.MaxId} уже существует.");

            var admin = new Admin
            {
                MaxId = request.MaxId,
                IsSuperAdmin = request.IsSuperAdmin,
                Username = request.UserName,
            };
            await _adminRepository.AddAsync(admin, cancellationToken);
            await _adminRepository.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
