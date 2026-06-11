using NivoMaxBot.Domain.Constants;
using NivoMaxBot.Messaging.Abstractions.Attachments.Inline;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.Routing;

namespace NivoMaxBot.Presentation.Handlers.Callbacks.WarrantyRequests.Admin.View
{
    [CallbackRoute(AdminWarrantyRequestsRoutes.List)]
    public class RepairListHandler
    {
        private readonly IMessengerClient _botClient;

        public RepairListHandler(
            IMessengerClient telegramBotClient)
        {
            _botClient = telegramBotClient;
        }
        public async Task HandleAsync(ICallbackQuery query, CancellationToken ct)
        {
            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new[] { new InlineKeyboardButton(
                    $"🆕 {WarrantyRequestStatus.New}", 
                    $"{AdminWarrantyRequestsRoutes.Filter}:{WarrantyRequestStatus.New}") },
                new[] { new InlineKeyboardButton(
                    $"✅ {WarrantyRequestStatus.Approved}",
                    $"{AdminWarrantyRequestsRoutes.Filter}:{WarrantyRequestStatus.Approved}") },
                new[] { new InlineKeyboardButton(
                    $"⚙️ {WarrantyRequestStatus.Processing}", 
                    $"{AdminWarrantyRequestsRoutes.Filter}:{WarrantyRequestStatus.Processing}") },
                new[] { new InlineKeyboardButton(
                    $"🏁 {WarrantyRequestStatus.Completed}", 
                    $"{AdminWarrantyRequestsRoutes.Filter}:{WarrantyRequestStatus.Completed}") },
                new[] { new InlineKeyboardButton(
                    $"❌ {WarrantyRequestStatus.Canceled}", 
                    $"{AdminWarrantyRequestsRoutes.Filter}:{WarrantyRequestStatus.Canceled}") },
                new[] { new InlineKeyboardButton("📋 Все", 
                    $"{AdminWarrantyRequestsRoutes.Filter}:all") },
                new[] { new InlineKeyboardButton("🔙 Назад", 
                    MenuRoutes.AdminMode) }
            });
            await _botClient.SendOrEditMessageAsync(query.Message.ChatId.Value, query.Message, 
                "Выберите статус заявок:", replyMarkup: keyboard, ct: ct);
        }
    }
}
