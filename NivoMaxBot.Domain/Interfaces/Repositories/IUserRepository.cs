using NivoMaxBot.Domain.Entities;
using NivoMaxBot.Domain.Interfaces.Repositories.Base;

namespace NivoMaxBot.Domain.Interfaces.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByMaxIdAsync(long telegramId, CancellationToken cancellationToken = default);
    }
}
