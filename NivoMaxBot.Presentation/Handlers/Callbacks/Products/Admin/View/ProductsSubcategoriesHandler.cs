using MediatR;
using NivoMaxBot.Application.Features.Categories.Queries;
using NivoMaxBot.Application.Features.Categories.Queries.ById;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.Routing;
using NivoMaxBot.Presentation.Common.Keyboards.Admin.Product;
using NivoMaxBot.Presentation.Common.Keyboards.Menu;

namespace NivoMaxBot.Presentation.Handlers.Callbacks.Products.Admin.View
{
    [CallbackRoute("products:subcategories:{parentId:int}")]
    public class ProductsSubcategoriesHandler
    {
        private readonly IMediator _mediator;
        private readonly IMessengerClient _botClient;
        private readonly IProductCategoryChoiceKeyboardFactory _keyboardFactory;

        public ProductsSubcategoriesHandler(
            IMediator mediator, 
            IMessengerClient botClient, 
            IProductCategoryChoiceKeyboardFactory keyboardFactory)
        {
            _mediator = mediator;
            _botClient = botClient;
            _keyboardFactory = keyboardFactory;
        }

        public async Task HandleAsync(ICallbackQuery query, int parentId, CancellationToken ct)
        {
            var chatId = query.Message.ChatId.Value;
            var categories = await _mediator.Send(new GetCategoriesQuery { ParentId = parentId }, ct);
            var parentCategory = await _mediator.Send(new GetCategoryByIdQuery { Id = parentId }, ct);
            string backCallback = parentCategory?.ParentId == null ? "admin:products" 
                : $"products:subcategories:{parentCategory.ParentId}";
            var keyboard = _keyboardFactory.CreateCategoryChoiceKeyboard(
                categories,
                parentId,
                true,
                backCallback,
                MenuType.Admin);
            await _botClient.SendOrEditMessageAsync(chatId, query.Message,
                $"Подкатегории:", replyMarkup: keyboard, ct: ct);
        }
    }
}
