using NivoMaxBot.Application.Interfaces;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.Routing;

namespace NivoMaxBot.Presentation.Handlers.Callbacks.Orders.Admin.UpdateStatus
{
    [CallbackRoute($"{AdminOrdersRoutes.CustomStatus}:{{orderId:int}}")]
    public class OrderCustomStatusHandler
    {
        private readonly IUserStateService _userStateService;
        private readonly IMessengerClient _botClient;

        public OrderCustomStatusHandler(
            IUserStateService userStateService,
            IMessengerClient messengerBotClient)
        {
            _userStateService = userStateService;
            _botClient = messengerBotClient;
        }
        public async Task HandleAsync(ICallbackQuery query, int orderId, CancellationToken ct)
        {
            var userId = query.From.Id;
            var userState = _userStateService.GetState(userId);
            userState.CurrentAction = "OrderCustomStatus";
            userState.Data["orderId"] = orderId;
            userState.Step = 1;
            _userStateService.SetState(userId, userState);
            await _botClient.SendOrEditMessageAsync(query.Message.ChatId.Value, query.Message,
                "Введите текст статуса (например, 'задерживается', 'будет готов через день'):", ct: ct);
        }
    }
}
