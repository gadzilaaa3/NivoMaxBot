using NivoMaxBot.Messaging.Abstractions.Attachments.Inline;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.Routing;

namespace NivoMaxBot.Presentation.Handlers.Callbacks.ConsultationRequests.Delete
{
    [CallbackRoute($"{ConsultationRequestRoutes.ConsultationDelete}:{{requestId:int}}")]
    public class ConsultationDeleteHandler
    {
        private readonly IMessengerClient _botClient;

        public ConsultationDeleteHandler(
            IMessengerClient botClient)
        {
            _botClient = botClient;
        }
        public async Task HandleAsync(ICallbackQuery query, int requestId, CancellationToken ct)
        {
            var keyboard = new InlineKeyboardMarkup(
            [
                [ new InlineKeyboardButton("✅ Да, удалить", 
                    $"{ConsultationRequestRoutes.ConsultationDeleteConfirm}:{requestId}") ],
                [ new InlineKeyboardButton("❌ Нет, отмена", 
                    $"{ConsultationRequestRoutes.ConsultationView}:{requestId}") ]
            ]);
            await _botClient.SendOrEditMessageAsync(query.Message.ChatId.Value, query.Message,
                $"Удалить заявку #{requestId}?", replyMarkup: keyboard, ct: ct);
        }
    }
}
