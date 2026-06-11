using Microsoft.EntityFrameworkCore;
using NivoMaxBot.Domain.Entities;
using NivoMaxBot.Domain.Interfaces.Repositories;
using NivoMaxBot.Infrastructure.Data;
using NivoMaxBot.Infrastructure.Repositories.Base;

namespace NivoMaxBot.Infrastructure.Repositories
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        public OrderRepository(AppDbContext context) : base(context) { }

        public async Task<bool> IsProductInAnyOrderAsync(int productId, 
            CancellationToken cancellationToken = default)
        {
            return await _context.OrderDetails.AnyAsync(od => od.ProductId == productId, 
                cancellationToken);
        }

        public async Task<Order?> GetByIdWithDetailsAsync(int orderId, 
            CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                .Include(o => o.Details)
                    .ThenInclude(d => d.ProductNavigation)
                .Include(o => o.UserNavigation)
                .FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);
        }

        public IQueryable<Order> GetUserOrdersQuery(int userId)
        {
            return _context.Orders
                .Include(o => o.Details)
                .Where(o => o.UserId == userId)
                .AsQueryable();
        }

        public async Task<IEnumerable<int>> GetUserIdsWithOrdersAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                .Select(o => o.UserId)
                .Distinct()
                .ToListAsync(cancellationToken);
        }

        public IQueryable<Order> GetOrdersQuery()
        {
            return _context.Orders
                .Include(o => o.Details)
                .Include(o => o.UserNavigation)
                .AsQueryable();
        }
    }
}
