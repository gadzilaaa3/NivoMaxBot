using NivoMaxBot.Application.Services;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.Routing;
using NivoMaxBot.Presentation.Services.User.WarrantyRequest;

namespace NivoMaxBot.Presentation.Handlers.Callbacks.User.Profile.WarrantyRequest
{
    [CallbackRoute(UserModeRoutes.Warranty)]
    public class ProfileWarrantyRequestsHandler
    {
        private readonly IWarrantyRequestsViewService _viewService;
        private readonly IUserService _userService;
        private readonly IMessengerClient _botClient;

        public ProfileWarrantyRequestsHandler(
            IWarrantyRequestsViewService viewService, 
            IUserService userService, 
            IMessengerClient botClient)
        {
            _viewService = viewService;
            _userService = userService;
            _botClient = botClient;
        }

        public async Task HandleAsync(ICallbackQuery query, CancellationToken ct)
        {
            var messengerId = query.From.Id;
            var user = await _userService.GetUserByMaxIdAsync(messengerId, ct);

            await _viewService.ShowRequestsListAsync(query.Message.ChatId.Value, query.Message, user.Id, 1, ct);
        }
    }
}
