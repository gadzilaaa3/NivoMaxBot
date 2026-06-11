using MediatR;
using NivoMaxBot.Application.Features.Categories.Queries.ById;
using NivoMaxBot.Application.Features.Products.Queries;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.Routing;
using NivoMaxBot.Presentation.Common.Keyboards.User.Catalog;
using NivoMaxBot.Shared.Pagination;

namespace NivoMaxBot.Presentation.Handlers.Callbacks.User.Catalog
{
    [CallbackRoute($"{UserCatalogRoutes.ProductsPage}:{{categoryId:int}}:{{pageNumber:int}}")]
    public class UserProductsPageHandler
    {
        private readonly IMediator _mediator;
        private readonly IUserProductListKeyboardFactory _keyboardFactory;
        private readonly IMessengerClient _botClient;

        public UserProductsPageHandler(
            IMediator mediator,
            IUserProductListKeyboardFactory keyboardFactory,
            IMessengerClient botClient)
        {
            _mediator = mediator;
            _keyboardFactory = keyboardFactory;
            _botClient = botClient;
        }

        public async Task HandleAsync(ICallbackQuery query, int categoryId, int pageNumber, CancellationToken ct)
        {
            var chatId = query.Message.ChatId.Value;
            var messageId = query.Message.MessageId;
            var pagedResult = await _mediator.Send(new GetProductsByCategoryPagedQuery
            {
                CategoryId = categoryId,
                PagedRequest = new PagedRequest { PageNumber = pageNumber, PageSize = 5 },
                IncludeUnavailable = false
            }, ct);

            var category = await _mediator.Send(new GetCategoryByIdQuery { Id = categoryId }, ct);
            
            var backCallback = category?.ParentId == null 
                ? UserCatalogRoutes.CatalogRoot
                : $"{UserCatalogRoutes.CategoryList}:{category.ParentId}";
            var keyboard = _keyboardFactory.CreateProductListKeyboard(pagedResult, categoryId, backCallback);
            
            var text = $"Товары в категории {category?.Name} (страница {pageNumber}):";
            await _botClient.SendOrEditMessageAsync(chatId, query.Message, text, keyboard, ct: ct);
        }
    }
}
