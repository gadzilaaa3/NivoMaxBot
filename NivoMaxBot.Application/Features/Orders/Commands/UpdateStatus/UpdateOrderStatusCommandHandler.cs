using MediatR;
using NivoMaxBot.Application.Common.Exceptions;
using NivoMaxBot.Application.Interfaces;
using NivoMaxBot.Domain.Entities;
using NivoMaxBot.Domain.Interfaces.Repositories;

namespace NivoMaxBot.Application.Features.Orders.Commands.UpdateStatus
{
    public class UpdateOrderStatusCommandHandler : IRequestHandler<UpdateOrderStatusCommand, bool>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly INotificationService _notificationService;
        private readonly IUserRepository _userRepository;

        public UpdateOrderStatusCommandHandler(
            IOrderRepository orderRepository,
            INotificationService notificationService,
            IUserRepository userRepository)
        {
            _orderRepository = orderRepository;
            _notificationService = notificationService;
            _userRepository = userRepository;
        }
        public async Task<bool> Handle(UpdateOrderStatusCommand request, CancellationToken ct)
        {
            var order = await _orderRepository.GetByIdWithDetailsAsync(request.OrderId, ct);
            if (order == null) throw new NotFoundException(nameof(Order), request.OrderId);
            if (order.Status == request.NewStatus) return true;

            order.Status = request.NewStatus;
            _orderRepository.Update(order);
            await _orderRepository.SaveChangesAsync(ct);
            var user = await _userRepository.GetByIdAsync(order.UserId, ct);
            if (user != null)
            {
                var message = $"📦 Статус вашего заказа #{order.Id} изменён на: *{request.NewStatus}*";
                await _notificationService.SendMessageToUserAsync(user.MaxId, message, ct);
            }
            return true;
        }
    }
}
