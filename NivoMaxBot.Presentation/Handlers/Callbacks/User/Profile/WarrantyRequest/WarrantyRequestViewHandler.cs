using NivoMaxBot.Application.Services;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.Routing;
using NivoMaxBot.Presentation.Services.User.WarrantyRequest;

namespace NivoMaxBot.Presentation.Handlers.Callbacks.User.Profile.WarrantyRequest
{
    [CallbackRoute($"{UserModeRoutes.WarrantyView}:{{requestId:int}}")]
    public class WarrantyRequestViewHandler
    {
        private readonly IWarrantyRequestsViewService _viewService;
        private readonly IUserService _userService;

        public WarrantyRequestViewHandler(
            IWarrantyRequestsViewService viewService, 
            IUserService userService)
        {
            _viewService = viewService;
            _userService = userService;
        }

        public async Task HandleAsync(ICallbackQuery query, int requestId, CancellationToken ct)
        {
            var messengerId = query.From.Id;
            var user = await _userService.GetUserByMaxIdAsync(messengerId, ct);

            await _viewService.ShowRequestDetailsAsync(query.Message.ChatId.Value, query.Message, requestId, user.Id, ct);
        }
    }
}
