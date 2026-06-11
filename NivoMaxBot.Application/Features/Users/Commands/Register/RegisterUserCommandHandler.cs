using MediatR;
using NivoMaxBot.Application.Common.Exceptions;
using NivoMaxBot.Domain.Entities;
using NivoMaxBot.Domain.Interfaces.Repositories;

namespace NivoMaxBot.Application.Features.Users.Commands.Register
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, int>
    {
        private readonly IUserRepository _userRepository;
        private readonly IBasketRepository _basketRepository;

        public RegisterUserCommandHandler(
            IUserRepository userRepository, 
            IBasketRepository basketRepository)
        {
            _userRepository = userRepository;
            _basketRepository = basketRepository;
        }

        public async Task<int> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            // Проверяем, не существует ли уже пользователь
            var existing = await _userRepository.GetByMaxIdAsync(request.MaxId, cancellationToken);
            if (existing != null)
                throw new BusinessRuleViolationException("Пользователь уже зарегистрирован.");

            // Создаём корзину
            var basket = new Domain.Entities.Basket();
            await _basketRepository.AddAsync(basket, cancellationToken);
            await _basketRepository.SaveChangesAsync(cancellationToken); // чтобы получить Id

            var user = new User
            {
                MaxId = request.MaxId,
                BasketId = basket.Id,
                BasketNavigation = basket, 
            };

            await _userRepository.AddAsync(user, cancellationToken);
            await _userRepository.SaveChangesAsync(cancellationToken);

            return user.Id;
        }
    }
}
