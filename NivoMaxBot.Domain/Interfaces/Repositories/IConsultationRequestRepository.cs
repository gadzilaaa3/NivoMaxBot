using NivoMaxBot.Domain.Entities;
using NivoMaxBot.Domain.Interfaces.Repositories.Base;

namespace NivoMaxBot.Domain.Interfaces.Repositories
{
    public interface IConsultationRequestRepository : IRepository<ConsultationRequest>
    {
        IQueryable<ConsultationRequest> GetQueryWithFilter(string? statusFilter);
        Task<ConsultationRequest?> GetByIdWithUserAsync(int id, 
            CancellationToken cancellationToken = default);
    }
}
