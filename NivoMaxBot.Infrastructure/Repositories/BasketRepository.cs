using Microsoft.EntityFrameworkCore;
using NivoMaxBot.Domain.Entities;
using NivoMaxBot.Domain.Interfaces.Repositories;
using NivoMaxBot.Infrastructure.Data;
using NivoMaxBot.Infrastructure.Repositories.Base;

namespace NivoMaxBot.Infrastructure.Repositories
{
    internal class BasketRepository : Repository<Basket>, IBasketRepository
    {
        public BasketRepository(AppDbContext context) : base(context) { }

        public void ClearBasket(int basketId)
        {
            var details = _context.BasketDetails.Where(d => d.BasketId == basketId);
            _context.BasketDetails.RemoveRange(details);
        }

        public async Task<Basket?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            var user = await _context.Users
                .Include(u => u.BasketNavigation)
                .ThenInclude(b => b.Details)
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

            return user?.BasketNavigation;
        }

        public async Task<Basket?> GetByUserIdWithDetailsAsync(int userId, CancellationToken cancellationToken = default)
        {
            var user = await _context.Users
                .Include(u => u.BasketNavigation)
                .ThenInclude(b => b.Details)
                .ThenInclude(bd => bd.ProductNavigation)
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

            return user?.BasketNavigation;
        }

        public async Task<BasketDetail?> GetDetailByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.BasketDetails.FindAsync([id], cancellationToken);
        }

        public async Task<bool> IsProductInAnyBasketAsync(int productId, 
            CancellationToken cancellationToken = default)
        {
            return await _context.BasketDetails.AnyAsync(bd => bd.ProductId == productId, cancellationToken);
        }

        public void RemoveDetail(BasketDetail detail)
        {
            _context.BasketDetails.Remove(detail);
        }

        public void UpdateDetail(BasketDetail detail)
        {
            _context.BasketDetails.Update(detail);
        }

        public IQueryable<BasketDetail> GetDetailsQuery(int basketId)
        {
            return _context.BasketDetails
                .Where(bd => bd.BasketId == basketId)
                .AsQueryable();
        }
    }
}
