using NivoMaxBot.Application.Services;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.Routing;
using NivoMaxBot.Presentation.Services.User.Order;

namespace NivoMaxBot.Presentation.Handlers.Callbacks.User.Profile.Order
{
    [CallbackRoute($"{UserModeRoutes.OrderView}:{{orderId:int}}")]
    public class OrderViewHandler
    {
        private readonly IOrdersViewService _ordersViewService;
        private readonly IUserService _userService;

        public OrderViewHandler(
            IOrdersViewService ordersViewService,
            IUserService userService)
        {
            _ordersViewService = ordersViewService;
            _userService = userService;
        }

        public async Task HandleAsync(ICallbackQuery query, int orderId, CancellationToken ct)
        {
            var messengerId = query.From.Id;
            var user = await _userService.GetUserByMaxIdAsync(messengerId, ct);

            await _ordersViewService.ShowOrderDetailsAsync(query.Message.ChatId.Value, 
                query.Message, orderId, user.Id, ct);
        }
    }
}
