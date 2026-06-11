using MediatR;
using NivoMaxBot.Application.Features.Basket.Commands.ClearCart;
using NivoMaxBot.Application.Services;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.Routing;
using NivoMaxBot.Presentation.Services.User.Cart;

namespace NivoMaxBot.Presentation.Handlers.Callbacks.User.Cart.Clear
{
    [CallbackRoute(UserModeRoutes.CartClear)]
    public class CartClearHandler
    {
        private readonly IMediator _mediator;
        private readonly IMessengerClient _botClient;
        private readonly IUserService _userService;
        private readonly ICartViewService _cartViewService;

        public CartClearHandler(
            IMediator mediator,
            IMessengerClient botClient,
            IUserService userService,
            ICartViewService cartViewService)
        {
            _botClient = botClient;
            _cartViewService = cartViewService;
            _mediator = mediator;
            _userService = userService;
        }

        public async Task HandleAsync(ICallbackQuery query, CancellationToken ct)
        {
            var messengerId = query.From.Id;
            var user = await _userService.GetUserByMaxIdAsync(messengerId, ct);
            if (user == null) return;

            await _mediator.Send(new ClearCartCommand { UserId = user.Id }, ct);
            await _botClient.AnswerCallbackQueryAsync(query.Id, "Корзина очищена", ct: ct);
            
            await RefreshCart(query.Message.ChatId.Value, query.Message, user.Id, ct);
        }

        private async Task RefreshCart(long chatId, IMessage message, int userId, CancellationToken ct)
        {
            await _cartViewService.ShowCart(chatId, message, 1, userId, ct);
        }
    }
}
