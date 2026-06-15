using NivoMaxBot.Application.Features.Broadcast.Commands;
using NivoMaxBot.Application.Interfaces;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.Routing;

namespace NivoMaxBot.Presentation.Handlers.Callbacks.Broadcast.Admin
{
    [CallbackRoute($"{AdminBroadcastRoutes.TypeHandler}:{{type}}")]
    public class BroadcastTypeHandler
    {
        private readonly IUserStateService _userStateService;
        private readonly IMessengerClient _botClient;

        public const string ActionName = "AwaitingBroadcastMessage";

        public BroadcastTypeHandler(
            IUserStateService userStateService,
            IMessengerClient messengerBotClient)
        {
            _userStateService = userStateService;
            _botClient = messengerBotClient;
        }

        public async Task HandleAsync(ICallbackQuery query, string type, CancellationToken ct)
        {
            var userId = query.From.Id;
            var state = _userStateService.GetState(userId);
            if (state.CurrentAction != AdminBroadcastHandler.ActionName)
            {
                await _botClient.AnswerCallbackQueryAsync(query.Id, "Ошибка состояния", ct: ct);
                return;
            }

            var broadcastType = type == "all" ? BroadcastType.All : BroadcastType.ActiveUsers;

            // Сохраняем тип в состоянии и переходим к ожиданию сообщения
            var broadcastData = new BroadcastData { BroadcastType = broadcastType };
            state.TypedData = broadcastData;
            state.CurrentAction = ActionName;
            _userStateService.SetState(userId, state);

            await _botClient.SendTextMessageAsync(query.Message.ChatId.Value, 
                "Отправьте сообщение (текст, фото, видео и т.д.), которое хотите разослать пользователям.", ct: ct);
        }

        public class BroadcastData
        {
            public string SourceMessageId { get; set; } = string.Empty;
            public long SourceChatId { get; set; }
            public BroadcastType BroadcastType { get; set; }
        }
    }
}
