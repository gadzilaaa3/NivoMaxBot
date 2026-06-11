using MediatR;
using NivoMaxBot.Application.Features.WarrantyRequest.Queries.ById;
using NivoMaxBot.Application.Features.WarrantyRequest.Queries.Paged;
using NivoMaxBot.Messaging.Abstractions.Attachments.Inline;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.Abstractions.Types.Enums;
using NivoMaxBot.Presentation.Common.Keyboards.Pagination;
using NivoMaxBot.Presentation.Handlers;
using NivoMaxBot.Shared.Helpers;
using NivoMaxBot.Shared.Pagination;

namespace NivoMaxBot.Presentation.Services.User.WarrantyRequest
{
    public class WarrantyRequestsViewService : IWarrantyRequestsViewService
    {
        private readonly IMediator _mediator;
        private readonly IMessengerClient _botClient;
        private readonly IPaginationControlsBuilder _paginationControlsBuilder;

        public WarrantyRequestsViewService(
            IMediator mediator,
            IMessengerClient botClient, 
            IPaginationControlsBuilder paginationService)
        {
            _mediator = mediator;
            _botClient = botClient;
            _paginationControlsBuilder = paginationService;
        }

        public async Task<(string Text, IInlineKeyboardMarkup Keyboard)> BuildRequestsListAsync
            (int userId, int pageNumber, CancellationToken ct)
        {
            var request = new PagedRequest { PageNumber = pageNumber, PageSize = 5 };
            var requests = await _mediator.Send(new GetUserWarrantyRequestsPagedQuery { UserId = userId, PagedRequest = request }, ct);

            if (!requests.Items.Any())
            {
                return ("У вас пока нет заявок на ремонт.",
                    new InlineKeyboardMarkup(new[] { new[] { new InlineKeyboardButton("🔙 Назад в профиль", "profile:main") } }));
            }

            var text = $"🔧 *Мои заявки (страница {requests.PageNumber} из {requests.TotalPages})*\n\n";
            foreach (var req in requests.Items)
            {
                text += $"#{req.Id} от {MoscowTimeHelper.ToMoscowTimeString(req.CreatedAt)} — {req.Status}\n";
                text += $"{req.ProblemDescription[..Math.Min(50, req.ProblemDescription.Length)]}...\n\n";
            }

            var buttons = new List<InlineKeyboardButton[]>();
            foreach (var req in requests.Items)
            {
                buttons.Add(new[] { new InlineKeyboardButton($"📄 Заявка #{req.Id}", $"warranty:view:{req.Id}") });
            }

            var paginationButtons = _paginationControlsBuilder.CreatePaginationButtons(requests, "warranty:page:{0}");
            buttons.AddRange(paginationButtons);
            buttons.Add(new[] { new InlineKeyboardButton("🔙 Назад в профиль", "profile:main") });

            var keyboard = new InlineKeyboardMarkup(buttons);
            return (text, keyboard);
        }

        public async Task<(string Text, IInlineKeyboardMarkup Keyboard)> BuildRequestDetailsAsync(int requestId, 
            int userId, CancellationToken ct)
        {
            var req = await _mediator.Send(new GetWarrantyRequestByIdQuery { Id = requestId }, ct);
            if (req == null)
                return ("Заявка не найдена.",
                    new InlineKeyboardMarkup(new[] { new[] { new InlineKeyboardButton("🔙 К списку", "profile:warranty") } }));

            var text = $"🔧 *Заявка #{req.Id}*\n" +
                       $"📅 Дата: {MoscowTimeHelper.ToMoscowTimeString(req.CreatedAt)}\n" +
                       $"📊 Статус: {req.Status}\n" +
                       $"🏢 ИНН: {req.INN ?? "—"}\n" +
                       $"🏙 Город: {req.City}\n" +
                       $"👤 Контактное лицо: {req.ContactPerson}\n" +
                       $"📞 Телефон: {req.ContactPhone}\n" +
                       $"📧 Email: {req.ContactEmail}\n" +
                       $"🔢 Серийный номер: {req.ProductSerialNumber}\n" +
                       $"📝 Описание проблемы: {req.ProblemDescription}";

            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new[] { new InlineKeyboardButton("🔙 К списку заявок", "profile:warranty") },
                new[] { new InlineKeyboardButton("🏠 Меню", MenuRoutes.UserMode) }
            });

            return (text, keyboard);
        }

        public async Task ShowRequestsListAsync(long chatId, IMessage? message,
            int userId, int pageNumber, CancellationToken ct)
        {
            var (text, keyboard) = await BuildRequestsListAsync(userId, pageNumber, ct);
            await _botClient.SendOrEditMessageAsync(chatId, 
                message, text, textFormat: TextFormat.Markdown, replyMarkup: keyboard, ct: ct);
        }

        public async Task ShowRequestDetailsAsync(long chatId, IMessage? message,
            int requestId, int userId, CancellationToken ct)
        {
            var (text, keyboard) = await BuildRequestDetailsAsync(requestId, userId, ct);
            await _botClient.SendOrEditMessageAsync(chatId, message, 
                text, textFormat: TextFormat.Markdown, replyMarkup: keyboard, ct: ct);
        }
    }
}
