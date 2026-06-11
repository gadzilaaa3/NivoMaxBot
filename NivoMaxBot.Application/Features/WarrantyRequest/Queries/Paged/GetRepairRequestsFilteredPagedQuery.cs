using MediatR;
using NivoMaxBot.Application.Features.WarrantyRequest.Dtos;
using NivoMaxBot.Shared.Pagination;

namespace NivoMaxBot.Application.Features.WarrantyRequest.Queries.Paged
{
    public class GetRepairRequestsFilteredPagedQuery : IRequest<PagedResult<WarrantyRequestDto>>
    {
        public string? StatusFilter { get; set; }
        public PagedRequest PagedRequest { get; set; } = new();
    }
}
