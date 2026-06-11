using MediatR;
using NivoMaxBot.Application.Features.Basket.Dtos;
using NivoMaxBot.Domain.Interfaces.Repositories;

namespace NivoMaxBot.Application.Features.Basket.Queries
{
    public class GetCartQueryHandler : IRequestHandler<GetCartQuery, IEnumerable<CartItemDto>>
    {
        private readonly IBasketRepository _basketRepository;

        public GetCartQueryHandler(IBasketRepository basketRepository)
        {
            _basketRepository = basketRepository;
        }

        public async Task<IEnumerable<CartItemDto>> Handle(GetCartQuery request, CancellationToken cancellationToken)
        {
            var basket = await _basketRepository.GetByUserIdWithDetailsAsync(request.UserId, cancellationToken);
            if (basket == null)
                return [];

            return basket.Details.Select(d => new CartItemDto
            {
                ProductId = d.ProductId,
                ProductName = d.ProductNavigation.Name,
                Price = d.ProductNavigation.Price,
                Quantity = d.ProductsQuantity
            }).ToList();
        }
    }
}
