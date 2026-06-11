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
    [CallbackRoute($"{AdminOrdersRoutes.Page}:{{pageNumber:int}}")]
    public class OrdersPageHandler
    {
        private readonly IMediator _mediator;
        private readonly IMessengerClient _botClient;
        private readonly IUserStateService _userStateService;
        private readonly IPaginationControlsBuilder _pagerControlsBuilder;

        public OrdersPageHandler(
            IMediator mediator,
            IMessengerClient telegramBotClient,
            IUserStateService userStateService,
            IPaginationControlsBuilder paginationControlsBuilder)
        {
            _mediator = mediator;
            _botClient = telegramBotClient;
            _userStateService = userStateService;
            _pagerControlsBuilder = paginationControlsBuilder;
        }

        public async Task HandleAsync(ICallbackQuery query, int pageNumber, CancellationToken ct)
        {
            var userId = query.From.Id;
            var userState = _userStateService.GetState(userId);
            var filter = userState.Data.ContainsKey("orderFilter") ? userState.Data["orderFilter"] as string : null;

            var pagedRequest = new PagedRequest { PageNumber = pageNumber, PageSize = 5 };
            var orders = await _mediator.Send(new GetOrdersFilteredPagedQuery { StatusFilter = filter, PagedRequest = pagedRequest }, ct);

            var buttons = new List<InlineKeyboardButton[]>();
            foreach (var o in orders.Items)
                buttons.Add([
                    new InlineKeyboardButton($"Заказ #{o.Id} от {MoscowTimeHelper.ToMoscowTimeString(o.CreatedAt)} - {o.Status}",
                        $"{AdminOrdersRoutes.View}:{o.Id}")
                ]);
            
            var paginationButtons = _pagerControlsBuilder.CreatePaginationButtons(orders, $"{AdminOrdersRoutes.Page}:{{0}}");
            buttons.AddRange(paginationButtons);
            buttons.Add(new[] { new InlineKeyboardButton("🔙 Выбрать другой статус", AdminOrdersRoutes.List) });
            var keyboard = new InlineKeyboardMarkup(buttons);

            var text = $"📦 *Заказы* (страница {orders.PageNumber} из {orders.TotalPages})";
            await _botClient.SendOrEditMessageAsync(query.Message.ChatId.Value, query.Message, 
                text, textFormat: TextFormat.Markdown, replyMarkup: keyboard, ct: ct);
        }
    }
}
