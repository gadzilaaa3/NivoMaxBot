using NivoMaxBot.Domain.Entities;
using NivoMaxBot.Domain.Interfaces.Repositories.Base;

namespace NivoMaxBot.Domain.Interfaces.Repositories
{
    public interface IAdminRepository : IRepository<Admin>
    {
        Task<Admin?> GetByMaxIdAsync(long telegramId, CancellationToken cancellationToken = default);

        Task<IEnumerable<Admin>> GetSuperAdminsAsync(CancellationToken cancellationToken = default);
    }
}
