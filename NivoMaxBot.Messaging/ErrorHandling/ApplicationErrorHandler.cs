using Microsoft.Extensions.Logging;
using NivoMaxBot.Application.Common.Exceptions;
using NivoMaxBot.Messaging.Abstractions.Attachments.Inline;
using NivoMaxBot.Messaging.Abstractions.Types;

namespace NivoMaxBot.Messaging.ErrorHandling
{
    public class ApplicationErrorHandler : IErrorHandler
    {
        private readonly IMessengerClient _messengerClient;
        private readonly ILogger<ApplicationErrorHandler> _logger;

        public ApplicationErrorHandler(IMessengerClient messengerClient, ILogger<ApplicationErrorHandler> logger)
        {
            _messengerClient = messengerClient;
            _logger = logger;
        }

        public Task HandleError(long chatId, Exception exception, CancellationToken ct)
            => HandleError(chatId, exception, null, ct);

        public async Task HandleError(long chatId, Exception exception, IInlineKeyboardMarkup? markup, CancellationToken ct)
        {
            string userMessage;

            switch (exception)
            {
                case FluentValidation.ValidationException validationEx:
                    // Собираем все ошибки валидации в одну строку
                    var errors = validationEx.Errors.Select(e => $"• {e.ErrorMessage}");
                    userMessage = "❌ Ошибка валидации:\n" + string.Join("\n", errors);
                    break;

                case NotFoundException notFoundEx:
                    userMessage = $"❌ {notFoundEx.Message}";
                    break;

                case BusinessRuleViolationException businessEx:
                    userMessage = $"❌ {businessEx.Message}";
                    break;

                default:
                    // Неожиданная ошибка – логируем и показываем общее сообщение
                    _logger.LogError(exception, "Unhandled exception");
                    userMessage = "❌ Произошла внутренняя ошибка. Пожалуйста, попробуйте позже.";
                    break;
            }

            await _messengerClient.SendTextMessageAsync(chatId, userMessage, markup, ct: ct);
        }
    }
}
