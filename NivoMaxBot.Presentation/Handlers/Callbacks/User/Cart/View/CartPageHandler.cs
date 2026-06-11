using MediatR;
using NivoMaxBot.Application.Features.Basket.Dtos;
using NivoMaxBot.Application.Features.Basket.Queries.Paged;
using NivoMaxBot.Application.Services;
using NivoMaxBot.Messaging.Abstractions.Attachments.Inline;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.Abstractions.Types.Enums;
using NivoMaxBot.Messaging.Routing;
using NivoMaxBot.Presentation.Common.Keyboards.Menu;
using NivoMaxBot.Presentation.Common.Keyboards.Pagination;
using NivoMaxBot.Shared.Pagination;

namespace NivoMaxBot.Presentation.Handlers.Callbacks.User.Cart.View
{
    [CallbackRoute($"{UserModeRoutes.CartPage}:{{pageNumber:int}}")]
    public class CartPageHandler
    {
        private readonly IMediator _mediator;
        private readonly IMessengerClient _botClient;
        private readonly IUserService _userService;
        private readonly IPaginationControlsBuilder _paginationService;
        private readonly IMenuBuilder _menuBuilder;

        public CartPageHandler(
            IMediator mediator,
            IMessengerClient botClient,
            IUserService userService,
            IPaginationControlsBuilder paginationService,
            IMenuBuilder menuBuilder)
        {
            _mediator = mediator;
            _botClient = botClient;
            _userService = userService;
            _paginationService = paginationService;
            _menuBuilder = menuBuilder;
        }

        public async Task HandleAsync(ICallbackQuery query, int pageNumber, CancellationToken ct)
        {
            var messengerId = query.From.Id;
            var user = await _userService.GetUserByMaxIdAsync(messengerId, ct);
            if (user == null) return;

            var chatId = query.Message.ChatId.Value;
            var messageId = query.Message.MessageId; // существующее сообщение

            var request = new PagedRequest { PageNumber = pageNumber, PageSize = 5 };
            var cartItems = await _mediator.Send(new GetCartPagedQuery { UserId = user.Id, PagedRequest = request }, ct);

            if (!cartItems.Items.Any())
            {
                await _botClient.SendOrEditMessageAsync(chatId, query.Message, "🛒 Корзина пуста.", ct: ct);
                return;
            }

            // Формируем текст
            var text = $"🛒 *Корзина (страница {cartItems.PageNumber} из {cartItems.TotalPages})*\n\n";
            decimal totalSum = 0;
            foreach (var item in cartItems.Items)
            {
                text += $"{item.ProductName} x {item.Quantity} = {item.Total} руб.\n";
                totalSum += item.Total;
            }
            text += $"\n*Итого на странице: {totalSum} руб.*";

            // Строим клавиатуру (кнопки управления)
            var buttons = BuildCartItemsKeyboard(cartItems, pageNumber);
            var keyboard = new InlineKeyboardMarkup(buttons);

            // Редактируем текущее сообщение
            await _botClient.SendOrEditMessageAsync(chatId, query.Message, text, 
                textFormat: TextFormat.Markdown, replyMarkup: keyboard, ct: ct);
        }

        private List<InlineKeyboardButton[]> BuildCartItemsKeyboard(PagedResult<CartItemDto> pagedResult, int page)
        {
            var buttons = new List<InlineKeyboardButton[]>();
            foreach (var item in pagedResult.Items)
            {
                buttons.Add(new[]
                {
                    new InlineKeyboardButton($"➖", $"{UserModeRoutes.CartUpdate}:{item.DetailId}:{item.Quantity - 1}"),
                    new InlineKeyboardButton($"{item.Quantity}", "noop"),
                    new InlineKeyboardButton($"➕", $"{UserModeRoutes.CartUpdate}:{item.DetailId}:{item.Quantity + 1}"),
                    new InlineKeyboardButton($"❌", $"{UserModeRoutes.CartUpdate}:{item.DetailId}")
                });
            }
            // Кнопки пагинации и очистки
            buttons.AddRange(_paginationService.CreatePaginationButtons(pagedResult, UserModeRoutes.CartPage + ":{0}"));
            buttons.Add(new[] { new InlineKeyboardButton("🗑 Очистить корзину", UserModeRoutes.CartClear) });
            buttons.Add(new[] { new InlineKeyboardButton("🔙 Назад", MenuRoutes.UserMode) });

            return buttons;
        }
    }
}
