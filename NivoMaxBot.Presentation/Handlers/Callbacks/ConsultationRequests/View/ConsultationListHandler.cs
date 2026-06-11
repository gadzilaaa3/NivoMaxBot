using NivoMaxBot.Domain.Constants;
using NivoMaxBot.Messaging.Abstractions.Attachments.Inline;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.Routing;

namespace NivoMaxBot.Presentation.Handlers.Callbacks.ConsultationRequests.View
{
    [CallbackRoute(ConsultationRequestRoutes.ConsultationList)]
    public class ConsultationListHandler
    {
        private readonly IMessengerClient _botClient;

        public ConsultationListHandler(
            IMessengerClient botClient)
        {
            _botClient = botClient;
        }
        public async Task HandleAsync(ICallbackQuery query, CancellationToken ct)
        {
            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new[] { new InlineKeyboardButton(
                    $"🆕 {ConsultationRequestStatus.New}", 
                    $"{ConsultationRequestRoutes.ConsultationFilter}:{ConsultationRequestStatus.New}") },
                new[] { new InlineKeyboardButton(
                    $"✅ {ConsultationRequestStatus.Completed}", 
                    $"{ConsultationRequestRoutes.ConsultationFilter}:{ConsultationRequestStatus.Completed}") },
                new[] { new InlineKeyboardButton(
                    $"❌ {ConsultationRequestStatus.Rejected}", 
                    $"{ConsultationRequestRoutes.ConsultationFilter}:{ConsultationRequestStatus.Rejected}") },
                new[] { new InlineKeyboardButton("📋 Все", $"{ConsultationRequestRoutes.ConsultationFilter}:all") },
                new[] { new InlineKeyboardButton("🔙 Назад", MenuRoutes.AdminMode) }
            });
            await _botClient.SendTextMessageAsync(query.Message.ChatId.Value, "Выберите статус заявок:", 
                replyMarkup: keyboard, ct: ct);
        }
    }
}
