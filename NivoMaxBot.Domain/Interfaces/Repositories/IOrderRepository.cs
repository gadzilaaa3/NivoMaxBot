using NivoMaxBot.Domain.Entities;
using NivoMaxBot.Domain.Interfaces.Repositories.Base;

namespace NivoMaxBot.Domain.Interfaces.Repositories
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<bool> IsProductInAnyOrderAsync(int productId, CancellationToken cancellationToken = default);

        Task<Order?> GetByIdWithDetailsAsync(int orderId, CancellationToken cancellationToken = default);

        IQueryable<Order> GetUserOrdersQuery(int userId);

        Task<IEnumerable<int>> GetUserIdsWithOrdersAsync(CancellationToken cancellationToken = default);

        IQueryable<Order> GetOrdersQuery();
    }
}
