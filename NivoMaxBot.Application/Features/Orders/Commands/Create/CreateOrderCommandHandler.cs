using MediatR;
using NivoMaxBot.Application.Common.Exceptions;
using NivoMaxBot.Domain.Entities;
using NivoMaxBot.Domain.Constants;
using NivoMaxBot.Domain.Interfaces.Repositories;
using NivoMaxBot.Application.Features.Users.Commands.Register;

namespace NivoMaxBot.Application.Features.Orders.Commands.Create
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, int>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IBasketRepository _basketRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMediator _mediator;

        public CreateOrderCommandHandler(
            IOrderRepository orderRepository,
            IBasketRepository basketRepository,
            IUserRepository userRepository,
            IMediator mediator)
        {
            _orderRepository = orderRepository;
            _basketRepository = basketRepository;
            _userRepository = userRepository;
            _mediator = mediator;
        }

        public async Task<int> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            // Создаем пользователя если его нет
            var user = await _userRepository.GetByMaxIdAsync(request.UserMaxId, cancellationToken); 
            if(user == null)
            {
                var id = await _mediator.Send(new RegisterUserCommand
                {
                    MaxId = request.UserMaxId
                });
                user = await _userRepository.GetByIdAsync(id);
            }

            var basket = await _basketRepository.GetByUserIdWithDetailsAsync(user.Id, cancellationToken);
            if (basket == null || basket.Details.Count == 0)
                throw new BusinessRuleViolationException("Корзина пуста.");

            // Создаём заказ
            var order = new Order
            {
                UserId = user.Id,
                Status = OrderStatus.New,
                ContactEmail = request.ContactEmail,
                ContactPhone = request.ContactPhone,
                CustomerName = request.CustomerName,
                INN = request.INN,
                Details = basket.Details.Select(d => new OrderDetail
                {
                    ProductId = d.ProductId,
                    ProductsQuantity = d.ProductsQuantity,
                    PriceAtOrder = d.ProductNavigation.Price // сохраняем цену на момент заказа
                }).ToList()
            };

            await _orderRepository.AddAsync(order, cancellationToken);
            await _orderRepository.SaveChangesAsync(cancellationToken);

            // Очищаем корзину
            _basketRepository.ClearBasket(basket.Id);
            await _basketRepository.SaveChangesAsync(cancellationToken);

            return order.Id;
        }
    }
}
