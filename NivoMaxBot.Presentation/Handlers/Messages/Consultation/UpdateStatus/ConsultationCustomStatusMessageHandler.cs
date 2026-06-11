using MediatR;
using Microsoft.Extensions.Logging;
using NivoMaxBot.Application.Features.ConsultationRequests.Commands.UpdateStatus;
using NivoMaxBot.Application.Interfaces;
using NivoMaxBot.Messaging.Abstractions.Attachments.Inline;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.ErrorHandling;
using NivoMaxBot.Messaging.Handlers.Interfaces;
using NivoMaxBot.Presentation.Common.Keyboards.Menu;
using NivoMaxBot.Presentation.Handlers.Callbacks.ConsultationRequests;

namespace NivoMaxBot.Presentation.Handlers.Messages.Consultation.UpdateStatus
{
    public class ConsultationCustomStatusMessageHandler : IMessageHandler
    {
        private readonly IUserStateService _userStateService;
        private readonly IMediator _mediator;
        private readonly IMessengerClient _botClient;
        private readonly ILogger<ConsultationCustomStatusMessageHandler> _logger;
        private readonly IMenuBuilder _menuBuilder;
        private readonly IErrorHandler _errorHandler;

        public ConsultationCustomStatusMessageHandler(
            IUserStateService userStateService,
            IMediator mediator,
            IMessengerClient botClient,
            ILogger<ConsultationCustomStatusMessageHandler> logger,
            IErrorHandler errorHandler,
            IMenuBuilder menuBuilder)
        {
            _userStateService = userStateService;
            _mediator = mediator;
            _botClient = botClient;
            _logger = logger;
        }

        public bool CanHandle(IMessage message)
        {
            var state = _userStateService.GetState(message.From.Id);
            return state.CurrentAction == "ConsultationCustomStatus" && state.Step == 1;
        }

        public async Task HandleAsync(IMessage message, CancellationToken ct)
        {
            var userId = message.From.Id;
            var chatId = message.ChatId.Value;
            var state = _userStateService.GetState(userId);
            var requestId = (int)state.Data["requestId"];
            var newStatus = message.Text;

            if (string.IsNullOrWhiteSpace(newStatus))
            {
                await _botClient.SendTextMessageAsync(chatId,
                    "Статус не может быть пустым. Попробуйте ещё раз:",
                    ct: ct);
                return;
            }

            var command = new UpdateConsultationRequestStatusCommand
            {
                RequestId = requestId,
                NewStatus = newStatus
            };

            try
            {
                await _mediator.Send(command, ct);
                await _botClient.SendTextMessageAsync(chatId,
                    $"✅ Статус заявки #{requestId} изменён на: {newStatus}",
                    ct: ct);
                _userStateService.ClearState(userId);

                // Возвращаемся к просмотру заявки
                var keyboard = new InlineKeyboardMarkup([
                    [ new InlineKeyboardButton("🔙 Вернуться к заявке",
                        $"{ConsultationRequestRoutes.ConsultationView}:{requestId}") ]
                ]);
                await _botClient.SendTextMessageAsync(chatId,
                    "Вернуться к просмотру заявки:",
                    replyMarkup: keyboard,
                    ct: ct);
            }
            catch (Exception ex)
            {
                var keyboard = _menuBuilder.AddControlButtons([], 
                    $"{ConsultationRequestRoutes.ConsultationView}:{requestId}", MenuType.Admin);
                await _errorHandler.HandleError(chatId, ex, keyboard, ct);
                _userStateService.ClearState(userId);
            }
        }
    }
}
