using MediatR;
using NivoMaxBot.Application.Features.Orders.Dtos;
using NivoMaxBot.Shared.Pagination;

namespace NivoMaxBot.Application.Features.Orders.Queries.Paged
{
    public class GetOrdersFilteredPagedQuery : IRequest<PagedResult<OrderDto>>
    {
        public string? StatusFilter { get; set; } // null – все, иначе точное совпадение
        public PagedRequest PagedRequest { get; set; } = new();
    }
}
