using MediatR;
using NivoMaxBot.Application.Features.ConsultationRequests.Queries.Paged;
using NivoMaxBot.Application.Interfaces;
using NivoMaxBot.Messaging.Abstractions.Attachments.Inline;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.Abstractions.Types.Enums;
using NivoMaxBot.Messaging.Routing;
using NivoMaxBot.Presentation.Common.Keyboards.Pagination;
using NivoMaxBot.Shared.Pagination;

namespace NivoMaxBot.Presentation.Handlers.Callbacks.ConsultationRequests.View
{
    [CallbackRoute($"{ConsultationRequestRoutes.ConsultationPage}:{{pageNumber:int}}")]
    public class ConsultationPageHandler
    {
        private readonly IMediator _mediator;
        private readonly IMessengerClient _botClient;
        private readonly IUserStateService _userStateService;
        private readonly IPaginationControlsBuilder _paginationService;

        public ConsultationPageHandler(
            IMediator mediator,
            IMessengerClient botClient,
            IUserStateService userStateService,
            IPaginationControlsBuilder paginationService)
        {
            _mediator = mediator;
            _botClient = botClient;
            _userStateService = userStateService;
            _paginationService = paginationService;
        }

        public async Task HandleAsync(ICallbackQuery query, int pageNumber, CancellationToken ct)
        {
            var userId = query.From.Id;
            var state = _userStateService.GetState(userId);
            var filter = state.Data.ContainsKey("consultationFilter")
                ? state.Data["consultationFilter"] as string
                : null;

            var pagedRequest = new PagedRequest { PageNumber = pageNumber, PageSize = 5 };
            var requests = await _mediator.Send(new GetConsultationRequestsPagedQuery
            {
                StatusFilter = filter,
                PagedRequest = pagedRequest
            }, ct);

            if (!requests.Items.Any())
            {
                await _botClient.SendOrEditMessageAsync(
                    query.Message.ChatId.Value,
                    query.Message,
                    "Нет заявок на данной странице.",
                    ct: ct);
                return;
            }

            var buttons = new List<InlineKeyboardButton[]>();
            foreach (var r in requests.Items)
            {
                buttons.Add(new[]
                {
                    new InlineKeyboardButton(
                        $"Заявка #{r.Id} от {r.CreatedAt:dd.MM.yyyy} - {r.Status}",
                        $"{ConsultationRequestRoutes.ConsultationView}:{r.Id}")
                });
            }

            var paginationButtons = _paginationService.CreatePaginationButtons(
                requests,
                $"{ConsultationRequestRoutes.ConsultationPage}:{{0}}");
            buttons.AddRange(paginationButtons);

            buttons.Add(new[]
            {
                new InlineKeyboardButton("🔙 Выбрать другой статус", ConsultationRequestRoutes.ConsultationList)
            });

            var keyboard = new InlineKeyboardMarkup(buttons);
            var text = $"📞 *Заявки на консультацию* (страница {requests.PageNumber} из {requests.TotalPages})";

            await _botClient.SendOrEditMessageAsync(
                query.Message.ChatId.Value,
                query.Message,
                text,
                textFormat: TextFormat.Markdown,
                replyMarkup: keyboard,
                ct: ct);
        }
    }
}
