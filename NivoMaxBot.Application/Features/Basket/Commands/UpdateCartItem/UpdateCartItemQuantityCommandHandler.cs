using MediatR;
using NivoMaxBot.Application.Common.Exceptions;
using NivoMaxBot.Domain.Entities;
using NivoMaxBot.Domain.Interfaces.Repositories;

namespace NivoMaxBot.Application.Features.Basket.Commands.UpdateCartItem
{
    public class UpdateCartItemQuantityCommandHandler : IRequestHandler<UpdateCartItemQuantityCommand, bool>
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IUserRepository _userRepository;

        public UpdateCartItemQuantityCommandHandler(
            IBasketRepository basketRepository,
            IUserRepository userRepository)
        {
            _basketRepository = basketRepository;
            _userRepository = userRepository;
        }

        public async Task<bool> Handle(UpdateCartItemQuantityCommand request, CancellationToken cancellationToken)
        {
            var detail = await _basketRepository.GetDetailByIdAsync(request.BasketDetailId, cancellationToken) 
                ?? throw new NotFoundException(nameof(BasketDetail), request.BasketDetailId);

            // Проверяем, что деталь принадлежит корзине пользователя
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            var basket = await _basketRepository.GetByUserIdAsync(request.UserId, cancellationToken);
            if(basket?.Id != user?.BasketId)
                throw new BusinessRuleViolationException("Нельзя изменять чужую корзину.");

            if (request.NewQuantity < 1)
            {
                // Если количество 0 или меньше, удаляем позицию
                _basketRepository.RemoveDetail(detail);
            }
            else
            {
                detail.ProductsQuantity = request.NewQuantity;
                _basketRepository.UpdateDetail(detail);
            }

            await _basketRepository.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
