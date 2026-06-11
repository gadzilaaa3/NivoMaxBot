using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.Routing;

namespace NivoMaxBot.Presentation.Handlers.Callbacks.User.ServiceSection
{
    [CallbackRoute(UserModeRoutes.ServiceDepartment)]
    public class ServiceDepartmentHandler
    {
        private readonly IMessengerClient _botClient;
        public ServiceDepartmentHandler(
            IMessengerClient botClient)
        {
            _botClient = botClient;
        }
        public async Task HandleAsync(ICallbackQuery query, CancellationToken ct)
        {
            await _botClient.AnswerCallbackQueryAsync(query.Id, "Данная функция находится в разработке",
                ct: ct);
        }
    }
}
