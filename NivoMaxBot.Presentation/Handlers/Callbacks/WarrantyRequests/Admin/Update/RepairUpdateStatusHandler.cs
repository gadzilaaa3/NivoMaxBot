using MediatR;
using NivoMaxBot.Application.Features.WarrantyRequest.Commands.Update;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.ErrorHandling;
using NivoMaxBot.Messaging.Routing;
using NivoMaxBot.Presentation.Common.Keyboards.Menu;
using NivoMaxBot.Presentation.Handlers.Callbacks.WarrantyRequests.Admin.View;

namespace NivoMaxBot.Presentation.Handlers.Callbacks.WarrantyRequests.Admin.Update
{
    [CallbackRoute($"{AdminWarrantyRequestsRoutes.UpdateStatus}:{{requestId:int}}:{{newStatus}}")]
    public class RepairUpdateStatusHandler
    {
        private readonly IMediator _mediator;
        private readonly IMessengerClient _botClient;
        private readonly IMenuBuilder _menuBuilder;
        private readonly IErrorHandler _errorHandler;

        public RepairUpdateStatusHandler(
            IMediator mediator,
            IMessengerClient telegramBotClient,
            IErrorHandler errorHandler,
            IMenuBuilder menuBuilder)
        {
            _mediator = mediator;
            _botClient = telegramBotClient;
            _menuBuilder = menuBuilder;
            _errorHandler = errorHandler;
        }
        public async Task HandleAsync(ICallbackQuery query, int requestId, string newStatus, CancellationToken ct)
        {
            var command = new UpdateWarrantyRequestStatusCommand { RequestId = requestId, NewStatus = newStatus };
            try
            {
                await _mediator.Send(command, ct);
                await _botClient.AnswerCallbackQueryAsync(query.Id, "✅ Статус обновлён", ct: ct);
                await new RepairViewHandler(_mediator, _botClient).HandleAsync(query, requestId, ct);
            }
            catch (Exception ex) 
            {
                var keyboard = _menuBuilder.AddControlButtons([], AdminWarrantyRequestsRoutes.List, MenuType.Admin);
                await _errorHandler.HandleError(query.Message.ChatId.Value, ex, keyboard, ct);
            }
        }
    }
}
