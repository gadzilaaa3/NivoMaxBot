using NivoMaxBot.Messaging.Routing;
using NivoMaxBot.Presentation.Services.MenuDisplay;
using NivoMaxBot.Messaging.Abstractions.Types;

namespace NivoMaxBot.Presentation.Handlers.Callbacks
{
    [CallbackRoute(MenuRoutes.UserMode)]
    public class UserModeHandler
    {
        private readonly IMenuDisplayService _menuDisplayService;

        public UserModeHandler(
            IMenuDisplayService menuDisplayService)
        {
            _menuDisplayService = menuDisplayService;
        }

        public async Task HandleAsync(ICallbackQuery query, CancellationToken ct)
        {
            var chatId = query.Message.ChatId.Value;

            await _menuDisplayService.ShowUserMenu(chatId, ct);
        }
    }
}
