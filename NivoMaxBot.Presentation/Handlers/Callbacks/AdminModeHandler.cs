using NivoMaxBot.Messaging.Routing;
using NivoMaxBot.Presentation.Services.MenuDisplay;
using NivoMaxBot.Messaging.Abstractions.Types;

namespace NivoMaxBot.Presentation.Handlers.Callbacks
{
    [CallbackRoute(MenuRoutes.AdminMode)]
    public class AdminModeHandler
    {
        private readonly IMenuDisplayService _menuDisplayService;
        
        public AdminModeHandler(
            IMenuDisplayService menuDisplayService)
        {
            _menuDisplayService = menuDisplayService;
        }

        public async Task HandleAsync(ICallbackQuery query, CancellationToken ct)
        {
            await _menuDisplayService.ShowAdminMenu(query.Message.ChatId.Value, ct);
        }
    }
}
