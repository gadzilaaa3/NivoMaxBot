using NivoMaxBot.Domain.Entities.Base;
using NivoMaxBot.Shared.Pagination;

namespace NivoMaxBot.Domain.Interfaces.Repositories.Base
{
    public interface IRepository<T> where T : BaseEntity
    {
        Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<IEnumerable<T>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default);

        Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<PagedResult<T>> GetAllPagedAsync(PagedRequest request, CancellationToken cancellationToken = default);

        Task AddAsync(T entity, CancellationToken cancellationToken = default);

        void Update(T entity);

        void Remove(T entity);

        Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);

        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
