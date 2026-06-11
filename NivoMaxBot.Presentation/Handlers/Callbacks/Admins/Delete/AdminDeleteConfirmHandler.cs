using MediatR;
using NivoMaxBot.Application.Features.Admins.Commands.Delete;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.ErrorHandling;
using NivoMaxBot.Messaging.Routing;
using NivoMaxBot.Presentation.Common.Keyboards.Menu;

namespace NivoMaxBot.Presentation.Handlers.Callbacks.Admins.Delete
{
    [CallbackRoute($"{AdminsRoutes.DeleteConfirm}:{{adminId:int}}")]
    public class AdminDeleteConfirmHandler
    {
        private readonly IMediator _mediator;
        private readonly IMessengerClient _botClient;
        private readonly IMenuBuilder _menuBuilder;
        private readonly IErrorHandler _errorHandler;

        public AdminDeleteConfirmHandler(
            IMediator mediator,
            IMessengerClient telegramBotClient,
            IMenuBuilder menuBuilder,
            IErrorHandler errorHandler)
        {
            _mediator = mediator;
            _botClient = telegramBotClient;
            _menuBuilder = menuBuilder;
            _errorHandler = errorHandler;
        }

        public async Task HandleAsync(ICallbackQuery query, int adminId, CancellationToken ct)
        {
            var keyboard = _menuBuilder.AddControlButtons([], AdminsRoutes.List, MenuType.Admin);

            try
            {
                var result = await _mediator.Send(new DeleteAdminCommand { AdminId = adminId }, ct);
                await _botClient.SendOrEditMessageAsync(query.Message.ChatId.Value, query.Message,
                    "✅ Администратор удалён.", replyMarkup: keyboard, ct: ct);
            }
            catch (Exception ex)
            {
                await _errorHandler.HandleError(query.Message.ChatId.Value, ex, keyboard, ct);
            }
        }
    }
}
