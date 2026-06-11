using Max.Bot;
using Max.Bot.Polling;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace NivoMaxBot.MaxMessaging.BackgroundServices
{
    public class MaxBotBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<MaxBotBackgroundService> _logger;

        public MaxBotBackgroundService(IServiceProvider serviceProvider, ILogger<MaxBotBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var botClient = scope.ServiceProvider.GetRequiredService<MaxClient>();
            var updateHandler = scope.ServiceProvider.GetRequiredService<IUpdateHandler>();

            await botClient.StartPollingAsync(updateHandler, cancellationToken: stoppingToken).ConfigureAwait(false);

            _logger.LogInformation("\nBot started receiving updates.\n");

            // Ожидаем сигнал остановки, чтобы keep-alive
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
    }
}
