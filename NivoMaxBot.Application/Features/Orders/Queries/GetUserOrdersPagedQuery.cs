using MediatR;
using NivoMaxBot.Application.Features.Orders.Dtos;
using NivoMaxBot.Shared.Pagination;

namespace NivoMaxBot.Application.Features.Orders.Queries
{
    public class GetUserOrdersPagedQuery : IRequest<PagedResult<OrderSummaryDto>>
    {
        public int UserId { get; set; }
        public PagedRequest PagedRequest { get; set; } = new();
    }
}
