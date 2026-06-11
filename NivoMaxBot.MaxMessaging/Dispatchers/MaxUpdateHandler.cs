using Max.Bot.Polling;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NivoMaxBot.MaxMessaging.Adapters;
using NivoMaxBot.Messaging.Abstractions.Types;

namespace NivoMaxBot.MaxMessaging.Dispatchers
{
    public class MaxUpdateHandler : IUpdateHandler
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<MaxUpdateHandler> _logger;

        public MaxUpdateHandler(IServiceProvider serviceProvider, ILogger<MaxUpdateHandler> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task HandleUpdateAsync(UpdateContext context, CancellationToken cancellationToken)
        {
            // Извлекаем оригинальный объект обновления от Max
            var maxUpdate = context.Update;

            // Если запрос пришел не из личного чата - пропускаем его
            var chatId = maxUpdate?.Message?.Recipient?.ChatId;
            if (chatId < 0)
            {
                return;
            }

            using var scope = _serviceProvider.CreateScope();
            var dispatcher = scope.ServiceProvider.GetRequiredService<IMessengerUpdateDispatcher>();
            // Преобразуем Max update в наш абстрактный IMessengerUpdate
            var messengerUpdate = new MaxUpdateAdapter(maxUpdate);

            await dispatcher.HandleAsync(messengerUpdate, cancellationToken);
        }

        public Task HandleCallbackQueryAsync(UpdateContext context, CancellationToken cancellationToken)
            => HandleUpdateAsync(context, cancellationToken);

        public Task HandleMessageAsync(UpdateContext context, CancellationToken cancellationToken)
            => HandleUpdateAsync(context, cancellationToken);

        public Task HandleUnknownUpdateAsync(UpdateContext context, CancellationToken cancellationToken)
            => HandleUpdateAsync(context, cancellationToken);
    }
}
