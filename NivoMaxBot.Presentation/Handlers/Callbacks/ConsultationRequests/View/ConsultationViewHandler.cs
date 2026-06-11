using MediatR;
using NivoMaxBot.Application.Features.ConsultationRequests.Queries.ById;
using NivoMaxBot.Domain.Constants;
using NivoMaxBot.Messaging.Abstractions.Attachments.Inline;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.Abstractions.Types.Enums;
using NivoMaxBot.Messaging.Routing;

namespace NivoMaxBot.Presentation.Handlers.Callbacks.ConsultationRequests.View
{
    [CallbackRoute($"{ConsultationRequestRoutes.ConsultationView}:{{requestId:int}}")]
    public class ConsultationViewHandler
    {
        private readonly IMediator _mediator;
        private readonly IMessengerClient _botClient;

        public ConsultationViewHandler(
            IMediator mediator,
            IMessengerClient botClient)
        {
            _mediator = mediator;
            _botClient = botClient;
        }

        public async Task HandleAsync(ICallbackQuery query, int requestId, CancellationToken ct)
        {
            var request = await _mediator.Send(new GetConsultationRequestByIdQuery { Id = requestId }, ct);
            if (request == null)
            {
                await _botClient.AnswerCallbackQueryAsync(query.Id, "Заявка не найдена", 
                    ct: ct);
                return;
            }
            var text = $"📞 *Заявка #{request.Id}*\n" +
                       $"👤 Имя: {request.CustomerName}\n" +
                       $"🏙 Город: {request.City}\n" +
                       $"📞 Телефон: {request.PhoneNumber}\n" +
                       $"📝 Описание: {request.Description ?? "—"}\n" +
                       $"📊 Статус: {request.Status}\n" +
                       $"🕒 Дата: {request.CreatedAt:dd.MM.yyyy HH:mm}";
            
            var buttons = new List<InlineKeyboardButton[]>();
            // Кнопки смены статуса
            var statuses = new[] { 
                ConsultationRequestStatus.New, 
                ConsultationRequestStatus.Completed,
                ConsultationRequestStatus.Rejected };
            foreach (var s in statuses)
            {
                if (s != request.Status)
                    buttons.Add(new[] { new InlineKeyboardButton($"📌 {s}", 
                        $"{ConsultationRequestRoutes.ConsultationUpdateStatus}:{requestId}:{s}") });
            }
            buttons.Add(new[] { new InlineKeyboardButton("✏️ Свой статус", 
                $"{ConsultationRequestRoutes.ConsultationCustomStatus}:{requestId}") });

            // Если статус "Завершена", добавляем кнопку удаления
            if (request.Status == ConsultationRequestStatus.Completed)
                buttons.Add(new[] { new InlineKeyboardButton("❌ Удалить", $"{ConsultationRequestRoutes.ConsultationDelete}:{requestId}") });
            buttons.Add(new[] { new InlineKeyboardButton("🔙 Назад", ConsultationRequestRoutes.ConsultationList) });
            var keyboard = new InlineKeyboardMarkup(buttons);
            await _botClient.SendOrEditMessageAsync(query.Message.ChatId.Value, 
                query.Message, text, textFormat: TextFormat.Markdown, replyMarkup: keyboard, ct: ct);
        }
    }
}
