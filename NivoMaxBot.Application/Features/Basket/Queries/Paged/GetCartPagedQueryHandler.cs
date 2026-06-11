using MediatR;
using Microsoft.EntityFrameworkCore;
using NivoMaxBot.Application.Common.Extensions;
using NivoMaxBot.Application.Features.Basket.Dtos;
using NivoMaxBot.Domain.Interfaces.Repositories;
using NivoMaxBot.Shared.Pagination;

namespace NivoMaxBot.Application.Features.Basket.Queries.Paged
{
    public class GetCartPagedQueryHandler : IRequestHandler<GetCartPagedQuery, PagedResult<CartItemDto>>
    {
        private readonly IBasketRepository _basketRepository;

        public GetCartPagedQueryHandler(IBasketRepository basketRepository) => _basketRepository = basketRepository;

        public async Task<PagedResult<CartItemDto>> Handle(GetCartPagedQuery request, 
            CancellationToken cancellationToken)
        {
            var basket = await _basketRepository.GetByUserIdAsync(request.UserId, cancellationToken);
            if (basket == null)
                return new PagedResult<CartItemDto> { Items = [], TotalCount = 0, 
                    PageNumber = 1, PageSize = request.PagedRequest.PageSize };

            var query = _basketRepository.GetDetailsQuery(basket.Id)
                .Include(d => d.ProductNavigation)
                .Select(d => new CartItemDto
                {
                    DetailId = d.Id,
                    ProductId = d.ProductId,
                    ProductName = d.ProductNavigation.Name,
                    PhotoMaxFileId = d.ProductNavigation.PhotoMaxFileId,
                    Price = d.ProductNavigation.Price,
                    Quantity = d.ProductsQuantity
                })
                .OrderBy(d => d.ProductName);

            var pagedResult = await query.ToPagedResultAsync(request.PagedRequest, cancellationToken);
            return pagedResult;
        }
    }
}
