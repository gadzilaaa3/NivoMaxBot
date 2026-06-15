using MediatR;
using NivoMaxBot.Application.Features.WarrantyRequest.Commands.Update;
using NivoMaxBot.Application.Interfaces;
using NivoMaxBot.Messaging.Abstractions.Attachments.Inline;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.ErrorHandling;
using NivoMaxBot.Messaging.Handlers.Interfaces;
using NivoMaxBot.Presentation.Common.Keyboards.Menu;
using NivoMaxBot.Presentation.Handlers.Callbacks.WarrantyRequests.Admin;

namespace NivoMaxBot.Presentation.Handlers.Messages.WarrantyRequest.Update
{
    public class RepairCustomStatusMessageHandler : IMessageHandler
    {
        private readonly IUserStateService _userStateService;
        private readonly IMediator _mediator;
        private readonly IMessengerClient _botClient;
        private readonly IMenuBuilder _menuBuilder;
        private readonly IErrorHandler _errorHandler;

        public RepairCustomStatusMessageHandler(
            IUserStateService userStateService,
            IMediator mediator,
            IMessengerClient messengerBotClient,
            IMenuBuilder menuBuilder,
            IErrorHandler errorHandler)
        {
            _userStateService = userStateService;
            _mediator = mediator;
            _botClient = messengerBotClient;
            _menuBuilder = menuBuilder;
            _errorHandler = errorHandler;
        }
        public bool CanHandle(IMessage message)
        {
            var s = _userStateService.GetState(message.From.Id);
            return s.CurrentAction == "RepairCustomStatus" && s.Step == 1;
        }
        public async Task HandleAsync(IMessage message, CancellationToken ct)
        {
            var userId = message.From.Id;
            var chatId = message.ChatId.Value;
            var s = _userStateService.GetState(userId);
            var requestId = (int)s.Data["requestId"];
            var newStatus = message.Text;
            if (string.IsNullOrWhiteSpace(newStatus))
            {
                await _botClient.SendTextMessageAsync(chatId, 
                    "Текст статуса не может быть пустым. Попробуйте ещё раз:", ct: ct);
                return;
            }
            var command = new UpdateWarrantyRequestStatusCommand { RequestId = requestId, NewStatus = newStatus };
            try
            {
                await _mediator.Send(command, ct);
                await _botClient.SendTextMessageAsync(chatId, 
                    $"✅ Статус заявки #{requestId} изменён на: {newStatus}", ct: ct);
                _userStateService.ClearState(userId);
                var keyboard = new InlineKeyboardMarkup([
                    [new InlineKeyboardButton("🔙 Вернуться к заявке", 
                        $"{AdminWarrantyRequestsRoutes.View}:{requestId}") ]
                ]);
                await _botClient.SendTextMessageAsync(chatId, "Вернуться к просмотру заявки:", 
                    replyMarkup: keyboard, ct: ct);
            }
            catch (Exception ex) 
            {
                var keyboard = _menuBuilder.AddControlButtons([], AdminWarrantyRequestsRoutes.List, MenuType.Admin);
                await _errorHandler.HandleError(chatId, ex, keyboard, ct);
            }
        }
    }
}
