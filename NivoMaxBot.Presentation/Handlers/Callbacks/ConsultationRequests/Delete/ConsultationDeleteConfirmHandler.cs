using MediatR;
using NivoMaxBot.Application.Features.ConsultationRequests.Commands.Delete;
using NivoMaxBot.Messaging.Abstractions.Attachments.Inline;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.ErrorHandling;
using NivoMaxBot.Messaging.Routing;
using NivoMaxBot.Presentation.Common.Keyboards.Menu;

namespace NivoMaxBot.Presentation.Handlers.Callbacks.ConsultationRequests.Delete
{
    [CallbackRoute($"{ConsultationRequestRoutes.ConsultationDeleteConfirm}:{{requestId:int}}")]
    public class ConsultationDeleteConfirmHandler
    {
        private readonly IMediator _mediator;
        private readonly IMessengerClient _botClient;
        private readonly IErrorHandler _errorHandler;
        private readonly IMenuBuilder _menuBuilder;

        public ConsultationDeleteConfirmHandler(
            IMediator mediator,
            IMessengerClient botClient,
            IErrorHandler errorHandler,
            IMenuBuilder menuBuilder)
        {
            _mediator = mediator;
            _botClient = botClient;
            _errorHandler = errorHandler;
            _menuBuilder = menuBuilder;
        }
        public async Task HandleAsync(ICallbackQuery query, int requestId, CancellationToken ct)
        {
            try
            {
                await _mediator.Send(new DeleteConsultationRequestCommand { RequestId = requestId }, ct);
                await _botClient.SendOrEditMessageAsync(query.Message.ChatId.Value, query.Message,
                    $"✅ Заявка #{requestId} удалена.", ct: ct);
                // Вернуться к списку
                var keyboard = new InlineKeyboardMarkup([
                    [ new InlineKeyboardButton("🔙 К списку", ConsultationRequestRoutes.ConsultationList)]
                ]);

                await _botClient.SendTextMessageAsync(query.Message.ChatId.Value, "Вернуться к списку заявок:", 
                    replyMarkup: keyboard, ct: ct);
            }
            catch (Exception ex)
            {
                var keyboard = _menuBuilder.AddControlButtons([], 
                    $"{ConsultationRequestRoutes.ConsultationView}:{requestId}", MenuType.Admin);
                await _errorHandler.HandleError(query.Message.ChatId.Value, ex, ct);
            }
        }
    }
}
