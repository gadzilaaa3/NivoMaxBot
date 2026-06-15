using NivoMaxBot.Messaging.Abstractions.Attachments.Inline;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.Routing;

namespace NivoMaxBot.Presentation.Handlers.Callbacks.WarrantyRequests.Admin.Delete
{
    [CallbackRoute($"{AdminWarrantyRequestsRoutes.Delete}:{{requestId:int}}")]
    public class RepairDeleteHandler
    {
        private readonly IMessengerClient _botClient;
        public RepairDeleteHandler(
            IMessengerClient messengerBotClient)
        {
            _botClient = messengerBotClient;
        }
        public async Task HandleAsync(ICallbackQuery query, int requestId, CancellationToken ct)
        {
            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new[] { new InlineKeyboardButton("✅ Да, удалить", $"{AdminWarrantyRequestsRoutes.DeleteConfirm}:{requestId}") },
                new[] { new InlineKeyboardButton("❌ Нет, отмена", $"{AdminWarrantyRequestsRoutes.View}:{requestId}") }
            });
            await _botClient.SendOrEditMessageAsync(query.Message.ChatId.Value, query.Message,
                $"Удалить заявку #{requestId}?", replyMarkup: keyboard, ct: ct);
        }
    }
}
