using NivoMaxBot.Application.Services;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.Abstractions.Types.Enums;
using NivoMaxBot.Messaging.Routing;
using NivoMaxBot.Presentation.Services.User.Order;

namespace NivoMaxBot.Presentation.Handlers.Callbacks.User.Profile.Order
{
    [CallbackRoute($"{UserModeRoutes.OrdersPage}:{{pageNumber:int}}")]
    public class OrdersPageHandler
    {
        private readonly IOrdersViewService _ordersViewService;
        private readonly IUserService _userService;
        private readonly IMessengerClient _botClient;

        public OrdersPageHandler(
            IOrdersViewService ordersViewService,
            IUserService userService,
            IMessengerClient botClient)
        {
            _ordersViewService = ordersViewService;
            _userService = userService;
            _botClient = botClient;
        }

        public async Task HandleAsync(ICallbackQuery query, int pageNumber, CancellationToken ct)
        {
            var messengerId = query.From.Id;
            var user = await _userService.GetUserByMaxIdAsync(messengerId, ct);

            var chatId = query.Message.ChatId.Value;
            var messageId = query.Message.MessageId;

            var (text, keyboard) = await _ordersViewService.BuildOrdersListAsync(user.Id, pageNumber, ct);
            await _botClient.SendOrEditMessageAsync(chatId, query.Message, 
                text, textFormat: TextFormat.Markdown, replyMarkup: keyboard, ct: ct);
        }
    }
}
