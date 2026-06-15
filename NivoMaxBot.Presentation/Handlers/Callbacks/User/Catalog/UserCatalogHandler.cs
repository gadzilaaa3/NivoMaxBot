using MediatR;
using NivoMaxBot.Application.Features.Categories.Queries;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.Routing;
using NivoMaxBot.Presentation.Common.Keyboards.User.Catalog;
using NivoMaxBot.Presentation.Services.MenuDisplay;

namespace NivoMaxBot.Presentation.Handlers.Callbacks.User.Catalog
{
    [CallbackRoute(UserCatalogRoutes.CatalogRoot)]
    public class UserCatalogHandler
    {
        private readonly IMediator _mediator;
        private readonly IUserCategoryKeyboardFactory _keyboardFactory;
        private readonly IMessengerClient _botClient;
        private readonly IMenuDisplayService _menuDisplayService;

        public UserCatalogHandler(
            IMediator mediator,
            IUserCategoryKeyboardFactory keyboardFactory,
            IMessengerClient messengerBotClient,
            IMenuDisplayService menuDisplayService)
        {
            _mediator = mediator;
            _keyboardFactory = keyboardFactory;
            _botClient = messengerBotClient;
            _menuDisplayService = menuDisplayService;
        }

        public async Task HandleAsync(ICallbackQuery query, CancellationToken ct)
        {
            var chatId = query.Message.ChatId.Value;
            var categories = await _mediator.Send(new GetCategoriesQuery { ParentId = null }, ct);
            var buttons = _keyboardFactory.CreateCategoriesListButtons(categories);

            await _menuDisplayService.ShowCatalogRoot(chatId, buttons, ct);
        }
    }
}
