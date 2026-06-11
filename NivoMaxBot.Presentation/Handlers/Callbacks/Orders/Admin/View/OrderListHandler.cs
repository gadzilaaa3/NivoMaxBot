using NivoMaxBot.Domain.Constants;
using NivoMaxBot.Messaging.Abstractions.Attachments.Inline;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.Routing;

namespace NivoMaxBot.Presentation.Handlers.Callbacks.Orders.Admin.View
{
    [CallbackRoute(AdminOrdersRoutes.List)]
    public class OrderListHandler
    {
        private readonly IMessengerClient _botClient;

        public OrderListHandler(
            IMessengerClient botClient)
        {
            _botClient = botClient;
        }
        public async Task HandleAsync(ICallbackQuery query, CancellationToken ct)
        {
            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new[] { new InlineKeyboardButton(
                    $"🆕 {OrderStatus.New}", 
                    $"{AdminOrdersRoutes.Filter}:{OrderStatus.New}") },
                new[] { new InlineKeyboardButton(
                    $"✅ {OrderStatus.Approved}", 
                    $"{AdminOrdersRoutes.Filter}:{OrderStatus.Approved}") },
                new[] { new InlineKeyboardButton(
                    $"📦 {OrderStatus.Sent}", 
                    $"{AdminOrdersRoutes.Filter}:{OrderStatus.Sent}") },
                new[] { new InlineKeyboardButton(
                    $"📬 {OrderStatus.Completed}", 
                    $"{AdminOrdersRoutes.Filter}:{OrderStatus.Completed}") },
                new[] { new InlineKeyboardButton(
                    $"❌ {OrderStatus.Canceled}", 
                    $"{AdminOrdersRoutes.Filter}:{OrderStatus.Canceled}") },
                new[] { new InlineKeyboardButton("📋 Все", $"{AdminOrdersRoutes.Filter}:all") },
                new[] { new InlineKeyboardButton("🔙 Назад", MenuRoutes.AdminMode) }
            });

            await _botClient.SendOrEditMessageAsync(query.Message.ChatId.Value, query.Message, 
                "Выберите статус заказов:", replyMarkup: keyboard, ct: ct);
        }
    }
}
