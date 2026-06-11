using MediatR;
using NivoMaxBot.Domain.Interfaces.Repositories;

namespace NivoMaxBot.Application.Features.Basket.Commands.ClearCart
{
    public class ClearCartCommandHandler : IRequestHandler<ClearCartCommand, bool>
    {
        private readonly IBasketRepository _basketRepository;

        public ClearCartCommandHandler(IBasketRepository basketRepository) => _basketRepository = basketRepository;

        public async Task<bool> Handle(ClearCartCommand request, CancellationToken cancellationToken)
        {
            var basket = await _basketRepository.GetByUserIdAsync(request.UserId, cancellationToken);
            if (basket == null)
                return true; // корзины нет, считаем успехом

            _basketRepository.ClearBasket(basket.Id);
            await _basketRepository.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
