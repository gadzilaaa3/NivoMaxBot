using MediatR;
using NivoMaxBot.Application.Features.Basket.Commands.UpdateCartItem;
using NivoMaxBot.Application.Services;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.Routing;
using NivoMaxBot.Presentation.Services.User.Cart;

namespace NivoMaxBot.Presentation.Handlers.Callbacks.User.Cart.Details.Update
{
    [CallbackRoute($"{UserModeRoutes.CartUpdate}:{{detailId:int}}:{{newQuantity:int}}")]
    public class CartUpdateHandler
    {
        private readonly IMediator _mediator;
        private readonly IMessengerClient _botClient;
        private readonly IUserService _userService;
        private readonly ICartViewService _cartViewService;

        public CartUpdateHandler(
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

        public async Task HandleAsync(ICallbackQuery query, int detailId, int newQuantity, CancellationToken ct)
        {
            var messengerId = query.From.Id;
            var user = await _userService.GetUserByMaxIdAsync(messengerId, ct);

            try
            {
                // Выполняем команду обновления количества
                await _mediator.Send(new UpdateCartItemQuantityCommand
                {
                    BasketDetailId = detailId,
                    NewQuantity = newQuantity,
                    UserId = user.Id
                }, ct);

                await _botClient.AnswerCallbackQueryAsync(query.Id, "✅ Количество обновлено", ct: ct);

                // Обновляем отображение корзины (первая страница)
                await _cartViewService.ShowCart(query.Message.ChatId.Value, query.Message, user.Id, 1, ct);
            }
            catch (Exception ex)
            {
                // Логирование ошибки
                await _botClient.AnswerCallbackQueryAsync(query.Id, "❌ Ошибка", ct: ct);
            }
        }
    }
}
