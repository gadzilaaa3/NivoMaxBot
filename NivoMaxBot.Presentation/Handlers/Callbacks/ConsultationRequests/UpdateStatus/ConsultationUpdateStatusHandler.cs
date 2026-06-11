using MediatR;
using NivoMaxBot.Application.Features.ConsultationRequests.Commands.UpdateStatus;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.ErrorHandling;
using NivoMaxBot.Messaging.Routing;
using NivoMaxBot.Presentation.Common.Keyboards.Menu;
using NivoMaxBot.Presentation.Handlers.Callbacks.ConsultationRequests.View;

namespace NivoMaxBot.Presentation.Handlers.Callbacks.ConsultationRequests.UpdateStatus
{
    [CallbackRoute($"{ConsultationRequestRoutes.ConsultationUpdateStatus}:{{requestId:int}}:{{newStatus}}")]
    public class ConsultationUpdateStatusHandler
    {
        private readonly IMediator _mediator;
        private readonly IMessengerClient _botClient;
        private readonly IMenuBuilder _menuBuilder;
        private readonly IErrorHandler _errorHandler;

        public ConsultationUpdateStatusHandler(
            IMediator mediator,
            IMessengerClient botClient,
            IMenuBuilder menuBuilder,
            IErrorHandler errorHandler)
        {
            _mediator = mediator;
            _botClient = botClient;
            _errorHandler = errorHandler;
            _menuBuilder = menuBuilder;
        }

        public async Task HandleAsync(ICallbackQuery query, int requestId, string newStatus, CancellationToken ct)
        {
            var command = new UpdateConsultationRequestStatusCommand { RequestId = requestId, NewStatus = newStatus };
            try
            {
                await _mediator.Send(command, ct);
                await _botClient.AnswerCallbackQueryAsync(query.Id, "Статус обновлён", 
                    ct: ct);
                // обновить отображение
                await new ConsultationViewHandler(_mediator, _botClient).HandleAsync(query, requestId, ct);
            }
            catch (Exception ex)
            {
                var keyboard = _menuBuilder.AddControlButtons([], 
                    $"{ConsultationRequestRoutes.ConsultationView}:{requestId}", MenuType.Admin);
                await _errorHandler.HandleError(query.Message.ChatId.Value, ex, keyboard, ct);
            }
        }
    }
}
