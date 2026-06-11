using MediatR;
using NivoMaxBot.Application.Features.Products.Dtos;
using NivoMaxBot.Shared.Pagination;

namespace NivoMaxBot.Application.Features.Products.Queries
{
    public class GetProductsByCategoryPagedQuery : IRequest<PagedResult<ProductDto>>
    {
        public int CategoryId { get; set; }

        public PagedRequest PagedRequest { get; set; } = new();

        public bool IncludeUnavailable { get; set; } = false;
    }
}
