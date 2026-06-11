using MediatR;
using NivoMaxBot.Application.Features.Basket.Commands.AddToBasket;
using NivoMaxBot.Application.Features.Users.Commands.Register;
using NivoMaxBot.Application.Interfaces;
using NivoMaxBot.Application.Services;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.Routing;

namespace NivoMaxBot.Presentation.Handlers.Callbacks.User.Cart.Details.Add
{
    [CallbackRoute($"{UserModeRoutes.CartAdd}:{{productId:int}}")]
    public class CartAddHandler
    {
        private readonly IMediator _mediator;
        private readonly IMessengerClient _botClient;
        private readonly IUserStateService _userStateService;
        private readonly IUserService _userService;

        public CartAddHandler(
            IMediator mediator, 
            IMessengerClient botClient, 
            IUserStateService userStateService, 
            IUserService userService)
        {
            _mediator = mediator;
            _botClient = botClient;
            _userStateService = userStateService;
            _userService = userService;
        }

        public async Task HandleAsync(ICallbackQuery query, int productId, CancellationToken ct)
        {
            var messengerId = query.From.Id;
            var chatId = query.Message.ChatId.Value;

            var user = await _userService.GetUserByMaxIdAsync(messengerId, ct);
            int userId;
            if (user == null)
            {
                var command = new RegisterUserCommand
                {
                    MaxId = messengerId,
                };

                userId = await _mediator.Send(command);
            }
            else
            {
                userId = user.Id;
            }

            var result = await _mediator.Send(new AddToCartCommand { UserId = userId, ProductId = productId }, ct);
            if (result)
            {
                await _botClient.AnswerCallbackQueryAsync(query.Id, "✅ Товар добавлен в корзину", ct: ct);
            }
            else
            {
                await _botClient.AnswerCallbackQueryAsync(query.Id, "❌ Не удалось добавить товар", ct: ct);
            }
        }
    }
}
