using MediatR;
using NivoMaxBot.Application.Features.Orders.Queries;
using NivoMaxBot.Messaging.Abstractions.Attachments.Inline;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.Abstractions.Types.Enums;
using NivoMaxBot.Presentation.Common.Keyboards.Menu;
using NivoMaxBot.Presentation.Common.Keyboards.Pagination;
using NivoMaxBot.Shared.Helpers;
using NivoMaxBot.Shared.Pagination;

namespace NivoMaxBot.Presentation.Services.User.Order
{
    public class OrdersViewService : IOrdersViewService
    {
        private readonly IMediator _mediator;
        private readonly IMessengerClient _botClient;
        private readonly IPaginationControlsBuilder _paginationService;
        private readonly IMenuBuilder _menuBuilder;

        public OrdersViewService(
            IMediator mediator,
            IMessengerClient botClient,
            IPaginationControlsBuilder paginationService,
            IMenuBuilder menuBuilder)
        {
            _mediator = mediator;
            _botClient = botClient;
            _paginationService = paginationService;
            _menuBuilder = menuBuilder;
        }

        /// <inheritdoc />
        public async Task<(string Text, IInlineKeyboardMarkup Keyboard)> BuildOrdersListAsync(int userId, int pageNumber, CancellationToken ct)
        {
            var request = new PagedRequest { PageNumber = pageNumber, PageSize = 5 };
            var orders = await _mediator.Send(new GetUserOrdersPagedQuery { UserId = userId, PagedRequest = request }, ct);

            if (!orders.Items.Any())
            {
                return ("У вас пока нет заказов.", new InlineKeyboardMarkup(new[]
                {
                    new[] { new InlineKeyboardButton("🔙 Назад в профиль", "profile:main") }
                }));
            }

            var text = $"📦 *Мои заказы (страница {orders.PageNumber} из {orders.TotalPages})*\n\n";
            foreach (var order in orders.Items)
            {
                text += $"#{order.Id} от {MoscowTimeHelper.ToMoscowTimeString(order.CreatedAt)} — {order.Status}\n";
                text += $"   {order.ItemsCount} товаров, сумма {order.TotalAmount} руб.\n\n";
            }

            var buttons = new List<InlineKeyboardButton[]>();

            // Кнопки для перехода к деталям каждого заказа
            foreach (var order in orders.Items)
            {
                buttons.Add(new[]
                {
                    new InlineKeyboardButton($"📄 Заказ #{order.Id}", $"user:order:view:{order.Id}")
                });
            }

            // Кнопки пагинации
            var paginationButtons = _paginationService.CreatePaginationButtons(orders, "user:orders:page:{0}");
            buttons.AddRange(paginationButtons);

            // Кнопка "Назад" в профиль (добавляется через MenuBuilder, но здесь можно добавить вручную)
            buttons.Add(new[] { new InlineKeyboardButton("🔙 Назад в профиль", "profile:main") });

            var keyboard = new InlineKeyboardMarkup(buttons);
            return (text, keyboard);
        }

        /// <inheritdoc />
        public async Task<(string Text, IInlineKeyboardMarkup Keyboard)> BuildOrderDetailsAsync(int orderId, int userId, CancellationToken ct)
        {
            var order = await _mediator.Send(new GetOrderByIdQuery { OrderId = orderId }, ct);
            if (order == null || order.UserId != userId)
            {
                return ("Заказ не найден.", new InlineKeyboardMarkup(new[]
                {
                    new[] { new InlineKeyboardButton("🔙 К списку заказов", "profile:orders") }
                }));
            }

            var text = $"📦 *Заказ #{order.Id}*\n" +
                       $"📅 Дата: {MoscowTimeHelper.ToMoscowTimeString(order.CreatedAt)}\n" +
                       $"📊 Статус: {order.Status}\n" +
                       $"👤 Получатель: {order.CustomerName}\n" +
                       $"📞 Телефон: {order.CustomerPhone}\n" +
                       $"📧 Email: {order.CustomerEmail ?? "—"}\n\n" +
                       $"*Состав заказа:*\n";

            foreach (var item in order.Items)
            {
                text += $"{item.ProductName} x {item.Quantity} = {item.Total} руб.\n";
            }
            text += $"\n*Итого: {order.TotalAmount} руб.*";

            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new[] { new InlineKeyboardButton("🔙 К списку заказов", "profile:orders") },
                new[] { new InlineKeyboardButton("🏠 Меню", "user_mode") }
            });

            return (text, keyboard);
        }

        /// <inheritdoc />
        public async Task ShowOrdersListAsync(long chatId, IMessage message, 
            int userId, int pageNumber, CancellationToken ct)
        {
            var (text, keyboard) = await BuildOrdersListAsync(userId, pageNumber, ct);
            await _botClient.SendOrEditMessageAsync(chatId, message, 
                text, textFormat: TextFormat.Markdown, replyMarkup: keyboard, ct: ct);
        }

        /// <inheritdoc />
        public async Task ShowOrderDetailsAsync(long chatId, IMessage message, 
            int orderId, int userId, CancellationToken ct)
        {
            var (text, keyboard) = await BuildOrderDetailsAsync(orderId, userId, ct);
            await _botClient.SendOrEditMessageAsync(chatId, message,
                text, textFormat: TextFormat.Markdown, replyMarkup: keyboard, ct: ct);
        }
    }
}
