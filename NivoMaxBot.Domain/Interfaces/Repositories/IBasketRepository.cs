using NivoMaxBot.Domain.Entities;
using NivoMaxBot.Domain.Interfaces.Repositories.Base;

namespace NivoMaxBot.Domain.Interfaces.Repositories
{
    public interface IBasketRepository : IRepository<Basket>
    {
        Task<bool> IsProductInAnyBasketAsync(int productId, CancellationToken cancellationToken = default);

        Task<Basket?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);

        void UpdateDetail(BasketDetail detail);

        void RemoveDetail(BasketDetail detail);

        void ClearBasket(int basketId);

        Task<Basket?> GetByUserIdWithDetailsAsync(int userId, CancellationToken cancellationToken = default);

        Task<BasketDetail?> GetDetailByIdAsync(int id, CancellationToken cancellationToken = default);

        public IQueryable<BasketDetail> GetDetailsQuery(int basketId);
    }
}
