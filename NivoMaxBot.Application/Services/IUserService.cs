using NivoMaxBot.Domain.Entities;

namespace NivoMaxBot.Application.Services
{
    public interface IUserService
    {
        Task<bool> IsAdminAsync(long maxId, CancellationToken cancellationToken = default);

        Task<User?> GetUserByMaxIdAsync(long maxId, CancellationToken cancellationToken = default);
    }
}
