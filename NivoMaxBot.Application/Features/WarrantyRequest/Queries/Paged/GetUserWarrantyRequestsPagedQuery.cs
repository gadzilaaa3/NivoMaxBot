using MediatR;
using NivoMaxBot.Application.Features.WarrantyRequest.Dtos;
using NivoMaxBot.Shared.Pagination;

namespace NivoMaxBot.Application.Features.WarrantyRequest.Queries.Paged
{
    public class GetUserWarrantyRequestsPagedQuery : IRequest<PagedResult<WarrantyRequestDto>>
    {
        public int UserId { get; set; }
        public PagedRequest PagedRequest { get; set; } = new();
    }
}
