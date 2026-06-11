using NivoMaxBot.Messaging.Abstractions.Attachments.Inline;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.Routing;

namespace NivoMaxBot.Presentation.Handlers.Callbacks.Orders.Admin.Delete
{
    [CallbackRoute($"{AdminOrdersRoutes.Delete}:{{orderId:int}}")]
    public class OrderDeleteHandler
    {
        private readonly IMessengerClient _botClient;
        public OrderDeleteHandler(IMessengerClient botClient)
        {
            _botClient = botClient;
        }
        public async Task HandleAsync(ICallbackQuery query, int orderId, CancellationToken ct)
        {
            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new[] { new InlineKeyboardButton("✅ Да, удалить", $"{AdminOrdersRoutes.DeleteConfirm}:{orderId}") },
                new[] { new InlineKeyboardButton("❌ Нет, отмена", $"{AdminOrdersRoutes.View}:{orderId}") }
            });
            await _botClient.SendOrEditMessageAsync(query.Message.ChatId.Value, query.Message,
                $"Удалить заказ #{orderId}?", replyMarkup: keyboard, ct: ct);
        }
    }
}
