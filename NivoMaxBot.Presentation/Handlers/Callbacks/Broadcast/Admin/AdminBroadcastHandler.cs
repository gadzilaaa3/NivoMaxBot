using NivoMaxBot.Application.Interfaces;
using NivoMaxBot.Messaging.Abstractions.Attachments.Inline;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.Routing;

namespace NivoMaxBot.Presentation.Handlers.Callbacks.Broadcast.Admin
{
    [CallbackRoute(AdminBroadcastRoutes.TypeSelection)]
    public class AdminBroadcastHandler
    {
        private readonly IUserStateService _userStateService;
        private readonly IMessengerClient _botClient;

        public const string ActionName = "BroadcastTypeSelection";

        public AdminBroadcastHandler(
            IUserStateService userStateService,
            IMessengerClient messengerBotClient)
        {
            _userStateService = userStateService;
            _botClient = messengerBotClient;
        }

        public async Task HandleAsync(ICallbackQuery query, CancellationToken ct)
        {
            var userId = query.From.Id;
            var state = _userStateService.GetState(userId);
            state.CurrentAction = ActionName;
            _userStateService.SetState(userId, state);

            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new[] { new InlineKeyboardButton("👥 Всем пользователям", 
                    $"{AdminBroadcastRoutes.TypeHandler}:all") },
                new[] { new InlineKeyboardButton("✅ Активным пользователям", 
                    $"{AdminBroadcastRoutes.TypeHandler}:active") },
                new[] { new InlineKeyboardButton("🔙 Отмена", MenuRoutes.AdminMode) }
            });

            await _botClient.SendOrEditMessageAsync(query.Message.ChatId.Value, query.Message,
                "Выберите целевую аудиторию:", replyMarkup: keyboard, ct: ct);
        }
    }
}
