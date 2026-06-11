using MediatR;
using NivoMaxBot.Application.Features.ConsultationRequests.Dtos;
using NivoMaxBot.Shared.Pagination;

namespace NivoMaxBot.Application.Features.ConsultationRequests.Queries.Paged
{
    public class GetConsultationRequestsPagedQuery 
        : IRequest<PagedResult<ConsultationRequestDto>>
    {
        public string? StatusFilter { get; set; }
        public PagedRequest PagedRequest { get; set; } = new();
    }
}
