using NivoMaxBot.Application.Services;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.Abstractions.Types.Enums;
using NivoMaxBot.Messaging.Routing;
using NivoMaxBot.Presentation.Services.User.WarrantyRequest;

namespace NivoMaxBot.Presentation.Handlers.Callbacks.User.Profile.WarrantyRequest
{
    [CallbackRoute($"{UserModeRoutes.WarrantyPage}:{{pageNumber:int}}")]
    public class WarrantyRequestsPageHandler
    {
        private readonly IWarrantyRequestsViewService _viewService;
        private readonly IUserService _userService;
        private readonly IMessengerClient _botClient;

        public WarrantyRequestsPageHandler(
            IWarrantyRequestsViewService viewService, 
            IUserService userService, 
            IMessengerClient botClient)
        {
            _viewService = viewService;
            _userService = userService;
            _botClient = botClient;
        }

        public async Task HandleAsync(ICallbackQuery query, int pageNumber, CancellationToken ct)
        {
            var messengerId = query.From.Id;
            var user = await _userService.GetUserByMaxIdAsync(messengerId, ct);
            if (user == null) return;

            var (text, keyboard) = await _viewService.BuildRequestsListAsync(user.Id, pageNumber, ct);
            await _botClient.SendOrEditMessageAsync(query.Message.ChatId.Value, 
                query.Message, text, textFormat: TextFormat.Markdown, replyMarkup: keyboard, ct: ct);
        }
    }
}
