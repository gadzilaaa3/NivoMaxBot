using Microsoft.EntityFrameworkCore;
using NivoMaxBot.Domain.Entities;
using NivoMaxBot.Domain.Interfaces.Repositories;
using NivoMaxBot.Infrastructure.Data;
using NivoMaxBot.Infrastructure.Repositories.Base;

namespace NivoMaxBot.Infrastructure.Repositories
{
    public class ConsultationRequestRepository 
        : Repository<ConsultationRequest>, IConsultationRequestRepository
    {
        public ConsultationRequestRepository(AppDbContext context) : base(context) { }

        public IQueryable<ConsultationRequest> GetQueryWithFilter(string? statusFilter)
        {
            var query = _context.ConsultationRequests.AsQueryable();
            if (!string.IsNullOrEmpty(statusFilter))
                query = query.Where(cr => cr.Status == statusFilter);
            return query;
        }

        public async Task<ConsultationRequest?> GetByIdWithUserAsync(int id, 
            CancellationToken cancellationToken = default)
        {
            return await _context.ConsultationRequests
                .Include(cr => cr.UserNavigation)
                .FirstOrDefaultAsync(cr => cr.Id == id, cancellationToken);
        }
    }
}
