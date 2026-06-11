using MediatR;
using NivoMaxBot.Application.Features.Orders.Queries;
using NivoMaxBot.Domain.Constants;
using NivoMaxBot.Messaging.Abstractions.Attachments.Inline;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.Abstractions.Types.Enums;
using NivoMaxBot.Messaging.Routing;
using NivoMaxBot.Shared.Helpers;

namespace NivoMaxBot.Presentation.Handlers.Callbacks.Orders.Admin.View
{
    [CallbackRoute($"{AdminOrdersRoutes.View}:{{orderId:int}}")]
    public class OrderViewHandler
    {
        private readonly IMediator _mediator;
        private readonly IMessengerClient _botClient;

        public OrderViewHandler(
            IMediator mediator,
            IMessengerClient telegramBotClient)
        {
            _mediator = mediator;
            _botClient = telegramBotClient;
        }

        public async Task HandleAsync(ICallbackQuery query, int orderId, CancellationToken ct)
        {
            var order = await _mediator.Send(new GetOrderByIdQuery { OrderId = orderId }, ct);
            if (order == null) 
            { 
                await _botClient.AnswerCallbackQueryAsync(query.Id, "Заказ не найден", ct: ct); 
                return; 
            }

            var text = $"📦 *Заказ #{order.Id}*\n" +
                $"👤 Клиент: {order.CustomerName}\n" +
                $"📞 Телефон: {order.CustomerPhone}\n" +
                $"📧 Email: {order.CustomerEmail ?? "—"}\n" +
                $"📅 Дата создания: {MoscowTimeHelper.ToMoscowTimeString(order.CreatedAt)}\n" +
                $"📊 Статус: {order.Status}\n" +
                $"\n*Состав заказа:*\n";

            foreach (var item in order.Items) text += $"{item.ProductName} x {item.Quantity} = {item.Total} руб.\n";
            text += $"\n*Итого: {order.TotalAmount} руб.*";
            var buttons = new List<InlineKeyboardButton[]>();

            var predefined = new[] { OrderStatus.New, OrderStatus.Approved,
                OrderStatus.Sent,
                OrderStatus.Completed, OrderStatus.Canceled };
            foreach (var s in predefined)
                if (s != order.Status)
                    buttons.Add(new[] { new InlineKeyboardButton($"📌 {s}", $"{AdminOrdersRoutes.UpdateStatus}:{orderId}:{s}") });
            buttons.Add(new[] { new InlineKeyboardButton("✏️ Свой статус", $"{AdminOrdersRoutes.CustomStatus}:{orderId}") });

            if (order.Status == OrderStatus.Completed)
            {
                buttons.Add(new[] { new InlineKeyboardButton("❌ Удалить", 
                    $"{AdminOrdersRoutes.Delete}:{orderId}") });
            }

            buttons.Add(new[] { new InlineKeyboardButton("🔙 К списку", AdminOrdersRoutes.List) });
            var keyboard = new InlineKeyboardMarkup(buttons);
            await _botClient.SendOrEditMessageAsync(query.Message.ChatId.Value, query.Message, 
                text, textFormat: TextFormat.Markdown, replyMarkup: keyboard, ct: ct);
        }
    }
}
