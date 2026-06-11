using MediatR;
using NivoMaxBot.Application.Features.Orders.Commands.UpdateStatus;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.ErrorHandling;
using NivoMaxBot.Messaging.Routing;
using NivoMaxBot.Presentation.Common.Keyboards.Menu;
using NivoMaxBot.Presentation.Handlers.Callbacks.Orders.Admin.View;

namespace NivoMaxBot.Presentation.Handlers.Callbacks.Orders.Admin.UpdateStatus
{
    [CallbackRoute($"{AdminOrdersRoutes.UpdateStatus}:{{orderId:int}}:{{newStatus}}")]
    public class OrderUpdateStatusHandler
    {
        private readonly IMediator _mediator;
        private readonly IMessengerClient _botClient;
        private readonly IErrorHandler _errorHandler;
        private readonly IMenuBuilder _menuBuilder;

        public OrderUpdateStatusHandler(
            IMediator mediator,
            IMessengerClient telegramBotClient,
            IErrorHandler errorHandler,
            IMenuBuilder menuBuilder)
        {
            _mediator = mediator;
            _botClient = telegramBotClient;
            _errorHandler = errorHandler;
            _menuBuilder = menuBuilder;
        }

        public async Task HandleAsync(ICallbackQuery query, int orderId, string newStatus, CancellationToken ct)
        {
            var command = new UpdateOrderStatusCommand { OrderId = orderId, NewStatus = newStatus };
            try
            {
                await _mediator.Send(command, ct);
                await _botClient.AnswerCallbackQueryAsync(query.Id, "✅ Статус обновлён", ct: ct);
                await new OrderViewHandler(_mediator, _botClient).HandleAsync(query, orderId, ct);
            }
            catch (Exception ex) 
            {
                var keyboard = _menuBuilder.AddControlButtons([], AdminOrdersRoutes.List, MenuType.Admin);
                await _errorHandler.HandleError(query.Message.ChatId.Value, ex, keyboard, ct);
            }
        }
    }
}
