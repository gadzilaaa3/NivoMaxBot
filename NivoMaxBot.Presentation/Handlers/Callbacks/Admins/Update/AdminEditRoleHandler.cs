using NivoMaxBot.Messaging.Abstractions.Attachments.Inline;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.Routing;

namespace NivoMaxBot.Presentation.Handlers.Callbacks.Admins.Update
{
    [CallbackRoute($"{AdminsRoutes.UpdateRole}:{{adminId:int}}")]
    public class AdminEditRoleHandler
    {
        private readonly IMessengerClient _botClient;

        public AdminEditRoleHandler(
            IMessengerClient botClient)
        {
            _botClient = botClient;
        }

        public async Task HandleAsync(ICallbackQuery query, int adminId, CancellationToken ct)
        {
            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new[] { new InlineKeyboardButton("🔹 Сделать суперадмином", 
                    $"{AdminsRoutes.SetRole}:{adminId}:super") },
                new[] { new InlineKeyboardButton("👤 Сделать обычным админом", 
                    $"{AdminsRoutes.SetRole}:{adminId}:regular") },
                new[] { new InlineKeyboardButton("🔙 Назад", $"{AdminsRoutes.View}:{adminId}") }
            });

            await _botClient.SendOrEditMessageAsync(query.Message.ChatId.Value, query.Message, 
                "Выберите новую роль:", replyMarkup: keyboard, ct: ct);
        }
    }
}
