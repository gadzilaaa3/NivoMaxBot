using MediatR;
using NivoMaxBot.Application.Features.Orders.Commands.Delete;
using NivoMaxBot.Messaging.Abstractions.Attachments.Inline;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.ErrorHandling;
using NivoMaxBot.Messaging.Routing;
using NivoMaxBot.Presentation.Common.Keyboards.Menu;

namespace NivoMaxBot.Presentation.Handlers.Callbacks.Orders.Admin.Delete
{
    [CallbackRoute($"{AdminOrdersRoutes.DeleteConfirm}:{{orderId:int}}")]
    public class OrderDeleteConfirmHandler
    {
        private readonly IMediator _mediator;
        private readonly IMessengerClient _botClient;
        private readonly IErrorHandler _errorHandler;
        private readonly IMenuBuilder _menuBuilder;

        public OrderDeleteConfirmHandler(
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
        public async Task HandleAsync(ICallbackQuery query, int orderId, CancellationToken ct)
        {
            try
            {
                await _mediator.Send(new DeleteOrderCommand { OrderId = orderId }, ct);
                await _botClient.SendOrEditMessageAsync(query.Message.ChatId.Value, query.Message,
                    $"✅ Заказ #{orderId} удалён.", ct: ct);
                // Предлагаем вернуться к списку заказов
                var keyboard = new InlineKeyboardMarkup([
                    [ new InlineKeyboardButton("🔙 К списку заказов", AdminOrdersRoutes.List) ]
                ]);
                await _botClient.SendTextMessageAsync(query.Message.ChatId.Value, 
                    "Вернуться к списку заказов:", replyMarkup: keyboard, ct: ct);
            }
            catch (Exception ex)
            {
                var keyboard = _menuBuilder.AddControlButtons([], $"{AdminOrdersRoutes.View}:{orderId}", MenuType.Admin);
                await _errorHandler.HandleError(query.Message.ChatId.Value, ex, ct);
            }
        }
    }
}
