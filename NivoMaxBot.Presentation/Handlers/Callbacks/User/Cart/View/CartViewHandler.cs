using NivoMaxBot.Application.Services;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.Routing;
using NivoMaxBot.Presentation.Services.User.Cart;

namespace NivoMaxBot.Presentation.Handlers.Callbacks.User.Cart.View
{
    [CallbackRoute(UserModeRoutes.CartView)]
    public class CartViewHandler
    {
        private readonly ICartViewService _cartViewService;
        private readonly IUserService _userService;
        private readonly IMessengerClient _botClient;

        public CartViewHandler(
            ICartViewService cartViewService, 
            IUserService userService,
            IMessengerClient botClient)
        {
            _cartViewService = cartViewService;
            _userService = userService;
            _botClient = botClient;
        }

        public async Task HandleAsync(ICallbackQuery query, CancellationToken ct)
        {
            var messengerId = query.From.Id;
            var user = await _userService.GetUserByMaxIdAsync(messengerId, ct);

            await _cartViewService.ShowCart(query.Message.ChatId.Value, query.Message, user.Id, 1, ct);
        }
    }
}
