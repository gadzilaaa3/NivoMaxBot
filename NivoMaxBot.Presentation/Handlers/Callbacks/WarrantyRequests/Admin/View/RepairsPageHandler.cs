using MediatR;
using NivoMaxBot.Application.Features.WarrantyRequest.Queries.Paged;
using NivoMaxBot.Application.Interfaces;
using NivoMaxBot.Messaging.Abstractions.Attachments.Inline;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.Abstractions.Types.Enums;
using NivoMaxBot.Messaging.Routing;
using NivoMaxBot.Presentation.Common.Keyboards.Pagination;
using NivoMaxBot.Shared.Helpers;
using NivoMaxBot.Shared.Pagination;

namespace NivoMaxBot.Presentation.Handlers.Callbacks.WarrantyRequests.Admin.View
{
    [CallbackRoute($"{AdminWarrantyRequestsRoutes.Page}:{{pageNumber:int}}")]
    public class RepairsPageHandler
    {
        private readonly IMediator _mediator;
        private readonly IMessengerClient _botClient;
        private readonly IUserStateService _userStateService;
        private readonly IPaginationControlsBuilder _paginationControlsBuilder;

        public RepairsPageHandler(
            IMediator mediator,
            IMessengerClient telegramBotClient,
            IUserStateService userStateService,
            IPaginationControlsBuilder paginationControlsBuilder)
        {
            _mediator = mediator;
            _botClient = telegramBotClient;
            _userStateService = userStateService;
            _paginationControlsBuilder = paginationControlsBuilder;
        }

        public async Task HandleAsync(ICallbackQuery query, int pageNumber, CancellationToken ct)
        {
            var userId = query.From.Id;
            var userState = _userStateService.GetState(userId);

            var filter = userState.Data.ContainsKey("repairFilter") ? userState.Data["repairFilter"] as string : null;
            var pagedRequest = new PagedRequest { PageNumber = pageNumber, PageSize = 5 };
            var repairs = await _mediator.Send(new GetRepairRequestsFilteredPagedQuery { StatusFilter = filter, PagedRequest = pagedRequest }, ct);
            
            var buttons = new List<InlineKeyboardButton[]>();
            foreach (var r in repairs.Items)
                buttons.Add([
                    new InlineKeyboardButton($"Заявка #{r.Id} от {MoscowTimeHelper.ToMoscowTimeString(r.CreatedAt)} - {r.Status}",
                        $"{AdminWarrantyRequestsRoutes.View}:{r.Id}")
                ]);
            
            var paginationButtons = _paginationControlsBuilder.CreatePaginationButtons(repairs, 
                $"{AdminWarrantyRequestsRoutes.Page}:{{0}}");
            buttons.AddRange(paginationButtons);
            buttons.Add([
                new InlineKeyboardButton("🔙 Выбрать другой статус", AdminWarrantyRequestsRoutes.List)
            ]);
            var keyboard = new InlineKeyboardMarkup(buttons);

            var text = $"🔧 *Заявки на ремонт* (страница {repairs.PageNumber} из {repairs.TotalPages})";
            await _botClient.SendOrEditMessageAsync(query.Message.ChatId.Value, query.Message, 
                text, textFormat: TextFormat.Markdown, replyMarkup: keyboard, ct: ct);
        }
    }
}
