using MediatR;
using NivoMaxBot.Application.Features.WarrantyRequest.Commands.Delete;
using NivoMaxBot.Messaging.Abstractions.Attachments.Inline;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.ErrorHandling;
using NivoMaxBot.Messaging.Routing;
using NivoMaxBot.Presentation.Common.Keyboards.Menu;

namespace NivoMaxBot.Presentation.Handlers.Callbacks.WarrantyRequests.Admin.Delete
{
    [CallbackRoute($"{AdminWarrantyRequestsRoutes.DeleteConfirm}:{{requestId:int}}")]
    public class RepairDeleteConfirmHandler
    {
        private readonly IMediator _mediator;
        private readonly IMessengerClient _botClient;
        private readonly IMenuBuilder _menuBuilder;
        private readonly IErrorHandler _errorHandler;

        public RepairDeleteConfirmHandler(
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
        public async Task HandleAsync(ICallbackQuery query, int requestId, CancellationToken ct)
        {
            try
            {
                await _mediator.Send(new DeleteWarrantyRequestCommand { RequestId = requestId }, ct);
                await _botClient.SendOrEditMessageAsync(query.Message.ChatId.Value, query.Message,
                    $"✅ Заявка #{requestId} удалена.", ct: ct);
                var keyboard = new InlineKeyboardMarkup([
                    [ new InlineKeyboardButton("🔙 К списку заявок", AdminWarrantyRequestsRoutes.List) ]
                ]);
                await _botClient.SendTextMessageAsync(query.Message.ChatId.Value, "Вернуться к списку заявок:", 
                    replyMarkup: keyboard, ct: ct);
            }
            catch (Exception ex)
            {
                var keyboard = _menuBuilder.AddControlButtons([], 
                    $"{AdminWarrantyRequestsRoutes.View}:{requestId}", MenuType.Admin);
                await _errorHandler.HandleError(query.Message.ChatId.Value, ex, keyboard, ct);
            }
        }
    }
}
