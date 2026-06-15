using NivoMaxBot.Application.Interfaces;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.Routing;

namespace NivoMaxBot.Presentation.Handlers.Callbacks.WarrantyRequests.Admin.Update
{
    [CallbackRoute($"{AdminWarrantyRequestsRoutes.CustomStatus}:{{requestId:int}}")]
    public class RepairCustomStatusHandler
    {
        private readonly IUserStateService _userStateService;
        private readonly IMessengerClient _botClient;

        public RepairCustomStatusHandler(
            IUserStateService userStateService,
            IMessengerClient messengerBotClient)
        {
            _userStateService = userStateService;
            _botClient = messengerBotClient;
        }
        public async Task HandleAsync(ICallbackQuery query, int requestId, CancellationToken ct)
        {
            var userId = query.From.Id;
            var userState = _userStateService.GetState(userId);
            userState.CurrentAction = "RepairCustomStatus";
            userState.Data["requestId"] = requestId;
            userState.Step = 1;
            _userStateService.SetState(userId, userState);
            await _botClient.SendOrEditMessageAsync(query.Message.ChatId.Value, query.Message,
                "Введите текст статуса (например, 'ожидает запчасти', 'мастер выехал'):", ct: ct);
        }
    }
}
