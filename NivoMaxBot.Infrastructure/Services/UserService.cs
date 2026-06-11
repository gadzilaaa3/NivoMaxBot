using NivoMaxBot.Application.Services;
using NivoMaxBot.Domain.Entities;
using NivoMaxBot.Domain.Interfaces.Repositories;

namespace NivoMaxBot.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly IAdminRepository _adminRepository;
        private readonly IUserRepository _userRepository;

        public UserService(IAdminRepository adminRepository, IUserRepository userRepository)
        {
            _adminRepository = adminRepository;
            _userRepository = userRepository;
        }

        public Task<User?> GetUserByMaxIdAsync(long maxId, CancellationToken cancellationToken = default)
        {
            return _userRepository.GetByMaxIdAsync(maxId, cancellationToken);
        }

        public async Task<bool> IsAdminAsync(long maxId, CancellationToken cancellationToken = default)
        {
            var admin = await _adminRepository.GetByMaxIdAsync(maxId, cancellationToken);
            return admin != null;
        }
    }
}
