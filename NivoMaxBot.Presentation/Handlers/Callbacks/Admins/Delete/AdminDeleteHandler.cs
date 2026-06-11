using NivoMaxBot.Messaging.Abstractions.Attachments.Inline;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.Routing;

namespace NivoMaxBot.Presentation.Handlers.Callbacks.Admins.Delete
{
    [CallbackRoute($"{AdminsRoutes.Delete}:{{adminId:int}}")]
    public class AdminDeleteHandler
    {
        private readonly IMessengerClient _botClient;

        public AdminDeleteHandler(
            IMessengerClient telegramBotClient)
        {
            _botClient = telegramBotClient;
        }

        public async Task HandleAsync(ICallbackQuery query, int adminId, CancellationToken ct)
        {
            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new[] { new InlineKeyboardButton("✅ Да", $"{AdminsRoutes.DeleteConfirm}:{adminId}") },
                new[] { new InlineKeyboardButton("❌ Нет", $"{AdminsRoutes.View}:{adminId}") }
            });

            await _botClient.SendOrEditMessageAsync(query.Message.ChatId.Value, query.Message,
                "Удалить этого администратора?", replyMarkup: keyboard, ct: ct);
        }
    }
}
