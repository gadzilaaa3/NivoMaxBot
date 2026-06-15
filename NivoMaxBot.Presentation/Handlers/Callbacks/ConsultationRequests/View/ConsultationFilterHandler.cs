using MediatR;
using NivoMaxBot.Application.Features.ConsultationRequests.Queries.Paged;
using NivoMaxBot.Application.Interfaces;
using NivoMaxBot.Messaging.Abstractions.Attachments.Inline;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.Abstractions.Types.Enums;
using NivoMaxBot.Messaging.Routing;
using NivoMaxBot.Presentation.Common.Keyboards.Pagination;
using NivoMaxBot.Shared.Helpers;
using NivoMaxBot.Shared.Pagination;

namespace NivoMaxBot.Presentation.Handlers.Callbacks.ConsultationRequests.View
{
    [CallbackRoute($"{ConsultationRequestRoutes.ConsultationFilter}:{{status}}")]
    public class ConsultationFilterHandler
    {
        private readonly IMediator _mediator;
        private readonly IMessengerClient _botClient;
        private readonly IUserStateService _userStateService;
        private readonly IPaginationControlsBuilder _paginationService;

        public ConsultationFilterHandler(
            IMediator mediator,
            IMessengerClient botClient,
            IUserStateService userStateService,
            IPaginationControlsBuilder controlsBuilder)
        {
            _mediator = mediator;
            _botClient = botClient;
            _userStateService = userStateService;
            _paginationService = controlsBuilder;
        }

        public async Task HandleAsync(ICallbackQuery query, string status, CancellationToken ct)
        {
            var filter = status == "all" ? null : status;
            var userId = query.From.Id;
            var state = _userStateService.GetState(userId);
            state.Data["consultationFilter"] = filter;
            state.Data["consultationPage"] = 1;
            _userStateService.SetState(userId, state);
            await ShowConsultationPage(query.Message.ChatId.Value, query.Message, filter, 1, ct);
        }

        private async Task ShowConsultationPage(long chatId, IMessage message, string? filter, int page, CancellationToken ct)
        {
            var pagedRequest = new PagedRequest { PageNumber = page, PageSize = 5 };
            var requests = await _mediator.Send(new GetConsultationRequestsPagedQuery 
            { 
                StatusFilter = filter, 
                PagedRequest = pagedRequest 
            }, ct);
            var buttons = new List<InlineKeyboardButton[]>();
            foreach (var r in requests.Items)
            {
                buttons.Add(new[] { new InlineKeyboardButton(
                    $"Заявка #{r.Id} от {MoscowTimeHelper.ToMoscowTimeString(r.CreatedAt)} - {r.Status}", 
                    $"{ConsultationRequestRoutes.ConsultationView}:{r.Id}") });
            }
            var paginationButtons = _paginationService.CreatePaginationButtons(requests, $"{ConsultationRequestRoutes.ConsultationPage}:{{0}}");
            buttons.AddRange(paginationButtons);
            buttons.Add(new[] { new InlineKeyboardButton("🔙 Выбрать другой статус", 
                ConsultationRequestRoutes.ConsultationList) });
            var keyboard = new InlineKeyboardMarkup(buttons);
            var text = $"📞 *Заявки на консультацию* (страница {requests.PageNumber} из {requests.TotalPages})";
            await _botClient.SendOrEditMessageAsync(chatId, message, text, 
                textFormat: TextFormat.Markdown, replyMarkup: keyboard, ct: ct);
        }
    }
}
