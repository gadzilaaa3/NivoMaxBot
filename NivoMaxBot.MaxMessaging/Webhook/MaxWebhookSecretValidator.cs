using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NivoMaxBot.MaxMessaging.Options;

namespace NivoMaxBot.MaxMessaging.Webhook
{
    public class MaxWebhookSecretValidator
    {
        private readonly MaxOptions _options;
        private readonly ILogger<MaxWebhookSecretValidator> _logger;

        public MaxWebhookSecretValidator(IOptions<MaxOptions> options, ILogger<MaxWebhookSecretValidator> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        public async Task<bool> ValidateRequestAsync(HttpRequest request)
        {
            // Если секрет не задан, пропускаем (но лучше всегда задавать)
            if (string.IsNullOrEmpty(_options.WebhookSecret))
            {
                _logger.LogWarning("Webhook secret is not configured. Skipping validation.");
                return true;
            }

            // Получаем заголовок с секретом (уточните название в документации MAX)
            // Возможные варианты: "X-Max-Bot-Api-Secret", "X-Max-Signature" и т.д.
            if (!request.Headers.TryGetValue("X-Max-Bot-Api-Secret", out var receivedSecret))
            {
                _logger.LogWarning("Missing webhook secret header.");
                return false;
            }

            // Сравниваем с конфигурацией (без учёта регистра)
            var isValid = string.Equals(receivedSecret, _options.WebhookSecret, StringComparison.OrdinalIgnoreCase);
            if (!isValid)
                _logger.LogWarning("Invalid webhook secret.");

            return isValid;
        }
    }
}
