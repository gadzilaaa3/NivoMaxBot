using NivoMaxBot.Domain.Entities;
using NivoMaxBot.Domain.Interfaces.Repositories.Base;

namespace NivoMaxBot.Domain.Interfaces.Repositories
{
    public interface IWarrantyRequestRepository : IRepository<WarrantyRequest>
    {
        IQueryable<WarrantyRequest> GetUserRequestsQuery(int userId);

        IQueryable<WarrantyRequest> GetWarrantyRequestsQuery();

        Task<IEnumerable<int>> GetUserIdsWithRequestsAsync(CancellationToken cancellationToken = default);

        Task<WarrantyRequest?> GetByIdWithUserAsync(int id, CancellationToken cancellationToken = default);
    }
}
