using Microsoft.EntityFrameworkCore;
using NivoMaxBot.Domain.Entities;
using NivoMaxBot.Domain.Interfaces.Repositories;
using NivoMaxBot.Infrastructure.Data;
using NivoMaxBot.Infrastructure.Repositories.Base;

namespace NivoMaxBot.Infrastructure.Repositories
{
    public class WarrantyRequestRepository : Repository<WarrantyRequest>, IWarrantyRequestRepository
    {
        public WarrantyRequestRepository(AppDbContext context) : base(context) { }

        public IQueryable<WarrantyRequest> GetUserRequestsQuery(int userId)
        {
            return _context.WarrantyRequests
                .Where(wr => wr.UserId == userId)
                .AsQueryable();
        }

        public async Task<IEnumerable<int>> GetUserIdsWithRequestsAsync(CancellationToken cancellationToken = default)
        {
            return await _context.WarrantyRequests
                .Select(w => w.UserId)
                .Distinct()
                .ToListAsync(cancellationToken);
        }

        public IQueryable<WarrantyRequest> GetWarrantyRequestsQuery()
        {
            return _context.WarrantyRequests
                .Include(wr => wr.UserNavigation)
                .AsQueryable();
        }

        public Task<WarrantyRequest?> GetByIdWithUserAsync(int id, CancellationToken ct)
        {
            return _context.WarrantyRequests
                .Include(r => r.UserNavigation)
                .FirstOrDefaultAsync(r => r.Id == id, ct);
        }
    }
}
