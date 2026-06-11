using MediatR;
using NivoMaxBot.Application.Common.Exceptions;
using NivoMaxBot.Domain.Entities;
using NivoMaxBot.Domain.Interfaces.Repositories;

namespace NivoMaxBot.Application.Features.Basket.Commands.RemoveCartItem
{
    public class RemoveCartItemCommandHandler : IRequestHandler<RemoveCartItemCommand, bool>
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IUserRepository _userRepository;

        public RemoveCartItemCommandHandler(
            IBasketRepository basketRepository,
            IUserRepository userRepository)
        {
            _basketRepository = basketRepository;
            _userRepository = userRepository;
        }

        public async Task<bool> Handle(RemoveCartItemCommand request, CancellationToken cancellationToken)
        {
            var detail = await _basketRepository.GetDetailByIdAsync(request.BasketDetailId, cancellationToken) 
                ?? throw new NotFoundException(nameof(BasketDetail), request.BasketDetailId);

            // Проверяем, что деталь принадлежит корзине пользователя
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            var basket = await _basketRepository.GetByUserIdAsync(request.UserId, cancellationToken);
            if (basket?.Id != user?.BasketId)
                throw new BusinessRuleViolationException("Нельзя изменять чужую корзину.");

            _basketRepository.RemoveDetail(detail);
            await _basketRepository.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
