using MediatR;
using NivoMaxBot.Application.Features.Categories.Queries.ById;
using NivoMaxBot.Application.Features.Products.Queries;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.Routing;
using NivoMaxBot.Presentation.Common.Keyboards.Admin.Product;
using NivoMaxBot.Presentation.Common.Keyboards.Menu;
using NivoMaxBot.Shared.Pagination;

namespace NivoMaxBot.Presentation.Handlers.Callbacks.Products.Admin.View
{
    [CallbackRoute($"{AdminProductRoutes.ProductsCategory}:{{categoryId:int}}")]
    public class ProductsCategoryHandler
    {
        private readonly IMediator _mediator;
        private readonly IMessengerClient _botClient;
        private readonly IProductListKeyboardFactory _keyboardFactory;

        public ProductsCategoryHandler(IMediator mediator, 
            IMessengerClient botClient, IProductListKeyboardFactory keyboardFactory)
        {
            _mediator = mediator;
            _botClient = botClient;
            _keyboardFactory = keyboardFactory;
        }

        public async Task HandleAsync(ICallbackQuery query, int categoryId, CancellationToken ct)
        {
            await ShowProductsPage(query, categoryId, 1, ct);
        }

        private async Task ShowProductsPage(ICallbackQuery query, int categoryId, int pageNumber, CancellationToken ct)
        {
            var chatId = query.Message.ChatId.Value;
            var pagedResult = await _mediator.Send(new GetProductsByCategoryPagedQuery
            {
                CategoryId = categoryId,
                PagedRequest = new PagedRequest { PageNumber = pageNumber, PageSize = 5 }, // размер страницы можно вынести в конфиг
                IncludeUnavailable = true,
            }, ct);

            var category = await _mediator.Send(new GetCategoryByIdQuery { Id = categoryId }, ct);
            var hasParent = category?.ParentId != null;
            var backCallback = category?.ParentId == null ? "admin:products" : $"products:subcategories:{category.ParentId}";

            var keyboard = _keyboardFactory.CreateProductListKeyboard(
                pagedResult,
                categoryId,
                categoryId, // currentParentId для кнопки "Подкатегории" – сама категория (покажем подкатегории этой категории)
                hasParent,
                backCallback,
                MenuType.Admin);

            var text = $"Товары в категории {category?.Name}:";
            await _botClient.SendOrEditMessageAsync(chatId, query.Message,
                text, replyMarkup: keyboard, ct: ct);
        }
    }
}
