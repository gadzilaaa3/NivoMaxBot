using NivoMaxBot.Application.Interfaces;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.Routing;

namespace NivoMaxBot.Presentation.Handlers.Callbacks.ConsultationRequests.UpdateStatus
{
    [CallbackRoute($"{ConsultationRequestRoutes.ConsultationCustomStatus}:{{requestId:int}}")]
    public class ConsultationCustomStatusHandler
    {
        private readonly IUserStateService _userStateService;
        private readonly IMessengerClient _botClient;

        public ConsultationCustomStatusHandler(
            IUserStateService userStateService, 
            IMessengerClient botClient)
        {
            _userStateService = userStateService;
            _botClient = botClient;
        }

        public async Task HandleAsync(ICallbackQuery query, int requestId, CancellationToken ct)
        {
            var userId = query.From.Id;
            var state = _userStateService.GetState(userId);
            state.CurrentAction = "ConsultationCustomStatus";
            state.Data["requestId"] = requestId;
            state.Step = 1;
            _userStateService.SetState(userId, state);

            await _botClient.SendTextMessageAsync(query.Message.ChatId.Value,
                "Введите текст нового статуса (например, 'Ожидание ответа клиента'):",
                ct: ct);
        }
    }
}
