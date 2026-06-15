using MediatR;
using NivoMaxBot.Application.Features.Orders.Queries.Paged;
using NivoMaxBot.Application.Interfaces;
using NivoMaxBot.Messaging.Abstractions.Attachments.Inline;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.Abstractions.Types.Enums;
using NivoMaxBot.Messaging.Routing;
using NivoMaxBot.Presentation.Common.Keyboards.Pagination;
using NivoMaxBot.Shared.Helpers;
using NivoMaxBot.Shared.Pagination;

namespace NivoMaxBot.Presentation.Handlers.Callbacks.Orders.Admin.View
{
    [CallbackRoute($"{AdminOrdersRoutes.Filter}:{{status}}")]
    public class OrderFilterHandler
    {
        private readonly IMediator _mediator;
        private readonly IMessengerClient _botClient;
        private readonly IUserStateService _userStateService;
        private readonly IPaginationControlsBuilder _paginationBuilder;

        public OrderFilterHandler(
            IMediator mediator,
            IMessengerClient messengerBotClient,
            IUserStateService userStateService,
            IPaginationControlsBuilder paginationControlsBuilder)
        {
            _mediator = mediator;
            _botClient = messengerBotClient;
            _userStateService = userStateService;
            _paginationBuilder = paginationControlsBuilder;
        }
        public async Task HandleAsync(ICallbackQuery query, string status, CancellationToken ct)
        {
            var userId = query.From.Id;
            var userState = _userStateService.GetState(userId);
            var filter = status == "all" ? null : status;
            userState.Data["orderFilter"] = filter;
            userState.Data["orderPage"] = 1;
            _userStateService.SetState(userId, userState);
            await ShowOrdersPage(query.Message.ChatId.Value, query.Message, filter, 1, ct);
        }
        private async Task ShowOrdersPage(long chatId, IMessage message, string? filter, int page, CancellationToken ct)
        {
            var pagedRequest = new PagedRequest { PageNumber = page, PageSize = 5 };
            var orders = await _mediator.Send(new GetOrdersFilteredPagedQuery { StatusFilter = filter, PagedRequest = pagedRequest }, ct);
            
            var buttons = new List<InlineKeyboardButton[]>();
            foreach (var o in orders.Items)
                buttons.Add([
                    new InlineKeyboardButton($"Заказ #{o.Id} от {MoscowTimeHelper.ToMoscowTimeString(o.CreatedAt)} - {o.Status}",
                        $"{AdminOrdersRoutes.View}:{o.Id}")
                ]);

            var paginationButtons = _paginationBuilder
                .CreatePaginationButtons(orders, $"{AdminOrdersRoutes.Page}:{{0}}");
            buttons.AddRange(paginationButtons);
            buttons.Add(new[] { new InlineKeyboardButton("🔙 Выбрать другой статус", AdminOrdersRoutes.List) });
            var keyboard = new InlineKeyboardMarkup(buttons);

            var text = $"📦 *Заказы* (страница {orders.PageNumber} из {orders.TotalPages})";
            await _botClient.SendOrEditMessageAsync(chatId, message, 
                text, textFormat: TextFormat.Markdown, replyMarkup: keyboard, ct: ct);
        }
    }
}
