using Max.Bot;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NivoMaxBot.MaxMessaging.Options;

namespace NivoMaxBot.MaxMessaging.Webhook
{
    public class WebhookConfigurator : IHostedService
    {
        private readonly MaxClient _maxClient;
        private readonly MaxOptions _options;
        private readonly ILogger<WebhookConfigurator> _logger;

        public WebhookConfigurator(MaxClient maxClient, 
            IOptions<MaxOptions> options, ILogger<WebhookConfigurator> logger)
        {
            _maxClient = maxClient;
            _options = options.Value;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(_options.WebhookUrl))
            {
                _logger.LogWarning("Webhook URL not configured. Skipping webhook setup.");
                return;
            }

            try
            {
                _logger.LogInformation("Configuring webhook: {WebhookUrl}", _options.WebhookUrl);
                await _maxClient.ConfigureWebhookAsync(
                    url: _options.WebhookUrl,
                    secret: _options.WebhookSecret,
                    cancellationToken: cancellationToken);
                _logger.LogInformation("Webhook configured successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to configure webhook.");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            // Можно опционально удалить вебхук при остановке, но обычно не требуется
            return Task.CompletedTask;
        }
    }
}
