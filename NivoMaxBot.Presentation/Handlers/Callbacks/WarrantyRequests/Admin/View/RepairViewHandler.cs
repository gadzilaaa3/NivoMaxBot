using MediatR;
using NivoMaxBot.Application.Features.WarrantyRequest.Queries.ById;
using NivoMaxBot.Domain.Constants;
using NivoMaxBot.Messaging.Abstractions.Attachments.Inline;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.Abstractions.Types.Enums;
using NivoMaxBot.Messaging.Routing;
using NivoMaxBot.Shared.Helpers;

namespace NivoMaxBot.Presentation.Handlers.Callbacks.WarrantyRequests.Admin.View
{
    [CallbackRoute($"{AdminWarrantyRequestsRoutes.View}:{{requestId:int}}")]
    public class RepairViewHandler
    {
        private readonly IMediator _mediator;
        private readonly IMessengerClient _botClient;

        public RepairViewHandler(
            IMediator mediator,
            IMessengerClient telegramBotClient)
        {
            _mediator = mediator;
            _botClient = telegramBotClient;
        }
        public async Task HandleAsync(ICallbackQuery query, int requestId, CancellationToken ct)
        {
            var req = await _mediator.Send(new GetWarrantyRequestByIdQuery { Id = requestId }, ct);
            if (req == null) 
            { 
                await _botClient.AnswerCallbackQueryAsync(query.Id, "Заявка не найдена", ct: ct); 
                return; 
            }
            var text = 
                $"🔧 *Заявка #{req.Id}*\n" +
                $"👤 Контактное лицо: {req.ContactPerson}\n" +
                $"📞 Телефон: {req.ContactPhone}\n" +
                $"📧 Email: {req.ContactEmail ?? "—"}\n" +
                $"📅 Дата: {MoscowTimeHelper.ToMoscowTimeString(req.CreatedAt)}\n" +
                $"📊 Статус: {req.Status}\n\n" +
                $"📝 Проблема:\n" +
                $"{req.ProblemDescription}\n\n" +
                $"🔢 Серийный номер: {req.ProductSerialNumber ?? "—"}";

            var buttons = new List<InlineKeyboardButton[]>();
            var predefined = new[] { WarrantyRequestStatus.New, WarrantyRequestStatus.Approved,
                WarrantyRequestStatus.Processing,
                WarrantyRequestStatus.Completed, WarrantyRequestStatus.Canceled};
            
            foreach (var s in predefined)
                if (s != req.Status)
                    buttons.Add([
                        new InlineKeyboardButton($"📌 {s}",
                            $"{AdminWarrantyRequestsRoutes.UpdateStatus}:{requestId}:{s}")
                    ]);

            buttons.Add(new[] { new InlineKeyboardButton("✏️ Свой статус", 
                $"{AdminWarrantyRequestsRoutes.CustomStatus}:{requestId}") });
            
            if (req.Status == WarrantyRequestStatus.Completed)
                buttons.Add(new[] { new InlineKeyboardButton("❌ Удалить", 
                    $"{AdminWarrantyRequestsRoutes.Delete}:{requestId}") });
            
            buttons.Add(new[] { new InlineKeyboardButton("🔙 К списку", 
                AdminWarrantyRequestsRoutes.List) });
            var keyboard = new InlineKeyboardMarkup(buttons);
            await _botClient.SendOrEditMessageAsync(query.Message.ChatId.Value, query.Message, 
                text, textFormat: TextFormat.Markdown, replyMarkup: keyboard, ct: ct);
        }
    }
}
