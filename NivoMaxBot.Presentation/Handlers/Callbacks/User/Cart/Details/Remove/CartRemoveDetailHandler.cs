using MediatR;
using NivoMaxBot.Application.Features.Basket.Commands.RemoveCartItem;
using NivoMaxBot.Application.Services;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.Routing;
using NivoMaxBot.Presentation.Services.User.Cart;

namespace NivoMaxBot.Presentation.Handlers.Callbacks.User.Cart.Details.Remove
{
    [CallbackRoute($"{UserModeRoutes.CartRemove}:{{detailId:int}}")]
    public class CartRemoveDetailHandler
    {
        private readonly IMediator _mediator;
        private readonly IMessengerClient _botClient;
        private readonly IUserService _userService;
        private readonly ICartViewService _cartViewService;

        public CartRemoveDetailHandler(
            IMediator mediator,
            IMessengerClient botClient,
            IUserService userService,
            ICartViewService cartViewService)
        {
            _mediator = mediator;
            _botClient = botClient;
            _userService = userService;
            _cartViewService = cartViewService;
        }

        public async Task HandleAsync(ICallbackQuery query, int detailId, CancellationToken ct)
        {
            var messengerId = query.From.Id;
            var user = await _userService.GetUserByMaxIdAsync(messengerId, ct);

            try
            {
                await _mediator.Send(new RemoveCartItemCommand
                {
                    BasketDetailId = detailId,
                    UserId = user.Id
                }, ct);
                await _botClient.AnswerCallbackQueryAsync(query.Id, "Товар удален", ct: ct);
                // После обновления можно показать корзину заново
                await RefreshCart(query.Message.ChatId.Value, query.Message, user.Id, ct);
            }
            catch (Exception ex)
            {
                await _botClient.AnswerCallbackQueryAsync(query.Id, "Ошибка", ct: ct);
            }
        }

        private async Task RefreshCart(long chatId, IMessage message, int userId, CancellationToken ct)
        {
            await _cartViewService.ShowCart(chatId, message, userId, 1, ct);
        }
    }
}
