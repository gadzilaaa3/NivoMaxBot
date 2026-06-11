using NivoMaxBot.Application.Services;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.Handlers.Interfaces;
using NivoMaxBot.Presentation.Services.MenuDisplay;

namespace NivoMaxBot.Presentation.Handlers.Messages
{
    public class StartMessageHandler : IMessageHandler
    {
        private readonly IUserService _userService;
        private readonly IMenuDisplayService _menuDisplayService;

        public StartMessageHandler( 
            IUserService userService,
            IMenuDisplayService menuDisplayService)
        {
            _userService = userService;
            _menuDisplayService = menuDisplayService;
        }

        public bool CanHandle(IMessage message) => message.Text == "/start";

        public async Task HandleAsync(IMessage message, CancellationToken cancellationToken)
        {
            var messangerId = message.From?.Id;

            if (messangerId == null)
            {
                return;
            }

            var isAdmin = await _userService.IsAdminAsync(messangerId.Value, cancellationToken);

            if (isAdmin)
            {
                await _menuDisplayService.ShowAdminStartMenu(message.ChatId.Value, cancellationToken);
            }
            else
            {
                await _menuDisplayService.ShowUserMenu(message.ChatId.Value, cancellationToken);
            }
        }
    }
}
