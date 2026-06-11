using MediatR;
using NivoMaxBot.Application.Common.Exceptions;
using NivoMaxBot.Domain.Constants;
using NivoMaxBot.Domain.Entities;
using NivoMaxBot.Domain.Interfaces.Repositories;

namespace NivoMaxBot.Application.Features.Orders.Commands.Delete
{
    public class DeleteOrderCommandHandler : IRequestHandler<DeleteOrderCommand, bool>
    {
        private readonly IOrderRepository _orderRepository;
        public DeleteOrderCommandHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }
        public async Task<bool> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
            if (order == null)
                throw new NotFoundException(nameof(Order), request.OrderId);

            // Разрешаем удаление только заказов со статусом "Завершен"
            if (order.Status != OrderStatus.Completed)
                throw new BusinessRuleViolationException($"Удалить можно только заказы со статусом '{OrderStatus.Completed}'.");

            _orderRepository.Remove(order);
            await _orderRepository.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
