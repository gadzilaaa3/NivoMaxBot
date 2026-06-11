using MediatR;
using NivoMaxBot.Application.Features.Basket.Queries.Paged;
using NivoMaxBot.Messaging.Abstractions.Attachments.Inline;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.Abstractions.Types.Enums;
using NivoMaxBot.Presentation.Common.Keyboards.Menu;
using NivoMaxBot.Presentation.Common.Keyboards.Pagination;
using NivoMaxBot.Shared.Pagination;

namespace NivoMaxBot.Presentation.Services.User.Cart
{
    public class CartViewService : ICartViewService
    {
        private readonly IMediator _mediator;
        private readonly IMessengerClient _botClient;
        private readonly IPaginationControlsBuilder _paginationService;
        private readonly IMenuBuilder _menuBuilder;

        public CartViewService(
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

        public async Task ShowCart(long chatId, IMessage? message, int userId, int pageNumber, CancellationToken ct)
        {
            var request = new PagedRequest { PageNumber = pageNumber, PageSize = 5 }; // размер страницы из конфига
            var cartItems = await _mediator.Send(new GetCartPagedQuery { UserId = userId, PagedRequest = request }, ct);

            if (cartItems.Items == null || !cartItems.Items.Any())
            {
                var btns = _menuBuilder.AddControlButtons([], null, MenuType.User);

                await _botClient.SendOrEditMessageAsync(chatId, message, 
                    "🛒 Корзина пуста.", replyMarkup: btns, ct: ct);
                return;
            }

            // Формируем текст для текущей страницы
            var text = $"🛒 *Корзина (страница {cartItems.PageNumber} из {cartItems.TotalPages})*\n\n";
            decimal totalSum = 0;
            foreach (var item in cartItems.Items)
            {
                text += $"{item.ProductName} x {item.Quantity} = {item.Total} руб.\n";
                totalSum += item.Total;
            }
            text += $"\n*Итого на странице: {totalSum} руб.*";

            // Создаём клавиатуру для каждого товара с кнопками управления
            var buttons = new List<InlineKeyboardButton[]>();
            foreach (var item in cartItems.Items)
            {
                buttons.Add(new[]
                {
                    new InlineKeyboardButton($"➖", $"user:cart:update:{item.DetailId}:{item.Quantity - 1}"),
                    new InlineKeyboardButton($"{item.Quantity}"),
                    new InlineKeyboardButton($"➕", $"user:cart:update:{item.DetailId}:{item.Quantity + 1}"),
                    new InlineKeyboardButton($"❌", $"user:cart:remove:{item.DetailId}"),
                });
            }

            // Кнопки пагинации
            var paginationButtons = _paginationService.CreatePaginationButtons(cartItems, "user:cart:page:{0}");
            buttons.AddRange(paginationButtons);

            // Кнопка очистки корзины
            buttons.Add(new[] { new InlineKeyboardButton("🗑 Очистить корзину", "user:cart:clear") });

            // Кнопка создания заказа
            buttons.Add(new[] { new InlineKeyboardButton("🛒 Оформить заказ", "user:order:create") });

            // Управляющие кнопки (назад в профиль, меню)
            var keyboard = _menuBuilder.AddControlButtons(buttons, null, MenuType.User);

            await _botClient.SendOrEditMessageAsync(chatId, message, 
                text, textFormat: TextFormat.Markdown, replyMarkup: keyboard, ct: ct);
        }
    }
}
