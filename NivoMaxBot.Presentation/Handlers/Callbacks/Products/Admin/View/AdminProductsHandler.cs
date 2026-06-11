using MediatR;
using NivoMaxBot.Application.Features.Categories.Queries;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.Routing;
using NivoMaxBot.Presentation.Common.Keyboards.Admin.Product;
using NivoMaxBot.Presentation.Common.Keyboards.Menu;

namespace NivoMaxBot.Presentation.Handlers.Callbacks.Products.Admin.View
{
    [CallbackRoute("admin:products")]
    public class AdminProductsHandler
    {
        private readonly IMediator _mediator;
        private readonly IMessengerClient _botClient;
        private readonly IProductCategoryChoiceKeyboardFactory _keyboardFactory;

        public AdminProductsHandler(
            IMediator mediator, 
            IMessengerClient botClient, 
            IProductCategoryChoiceKeyboardFactory keyboardFactory)
        {
            _mediator = mediator;
            _botClient = botClient;
            _keyboardFactory = keyboardFactory;
        }

        public async Task HandleAsync(ICallbackQuery query, CancellationToken ct)
        {
            var chatId = query.Message.ChatId.Value;
            var categories = await _mediator.Send(new GetCategoriesQuery { ParentId = null }, ct);
            var keyboard = _keyboardFactory.CreateCategoryChoiceKeyboard(
                categories,
                null,
                false,
                "admin_mode",
                MenuType.Admin);
            await _botClient.SendOrEditMessageAsync(chatId, query.Message,
                "Выберите категорию для управления товарами: ", 
                replyMarkup: keyboard, ct: ct);
        }
    }
}
