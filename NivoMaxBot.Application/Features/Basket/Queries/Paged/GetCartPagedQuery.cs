using MediatR;
using NivoMaxBot.Application.Features.Basket.Dtos;
using NivoMaxBot.Shared.Pagination;

namespace NivoMaxBot.Application.Features.Basket.Queries.Paged
{
    public class GetCartPagedQuery : IRequest<PagedResult<CartItemDto>>
    {
        public int UserId { get; set; }
        public PagedRequest PagedRequest { get; set; } = new();
    }
}
