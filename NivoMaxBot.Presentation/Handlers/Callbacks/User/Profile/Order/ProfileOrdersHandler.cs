using NivoMaxBot.Application.Services;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.Routing;
using NivoMaxBot.Presentation.Services.User.Order;

namespace NivoMaxBot.Presentation.Handlers.Callbacks.User.Profile.Order
{
    [CallbackRoute(UserModeRoutes.Orders)]
    public class ProfileOrdersHandler
    {
        private readonly IOrdersViewService _ordersViewService;
        private readonly IUserService _userService;
        private readonly IMessengerClient _botClient;

        public ProfileOrdersHandler(
            IOrdersViewService ordersViewService, 
            IUserService userService, 
            IMessengerClient botClient)
        {
            _ordersViewService = ordersViewService;
            _userService = userService;
            _botClient = botClient;
        }

        public async Task HandleAsync(ICallbackQuery query, CancellationToken ct)
        {
            var messengerId = query.From.Id;
            var user = await _userService.GetUserByMaxIdAsync(messengerId, ct);

            await _ordersViewService.ShowOrdersListAsync(query.Message.ChatId.Value, query.Message, user.Id, 1, ct);
        }
    }
}
