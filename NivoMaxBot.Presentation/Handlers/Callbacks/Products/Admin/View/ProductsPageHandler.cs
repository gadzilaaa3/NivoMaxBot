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
    [CallbackRoute("products:page:{categoryId:int}:{pageNumber:int}")]
    public class ProductsPageHandler
    {
        private readonly IMediator _mediator;
        private readonly IMessengerClient _botClient;
        private readonly IProductListKeyboardFactory _keyboardFactory;

        public ProductsPageHandler(
            IMediator mediator, 
            IMessengerClient botClient,
            IProductListKeyboardFactory keyboardFactory)
        {
            _mediator = mediator;
            _botClient = botClient;
            _keyboardFactory = keyboardFactory;
        }

        public async Task HandleAsync(ICallbackQuery query, int categoryId, int pageNumber, CancellationToken ct)
        {
            var chatId = query.Message.ChatId.Value;
            var pagedResult = await _mediator.Send(new GetProductsByCategoryPagedQuery
            {
                CategoryId = categoryId,
                PagedRequest = new PagedRequest { PageNumber = pageNumber, PageSize = 5 },
                IncludeUnavailable = true,
            }, ct);

            var category = await _mediator.Send(new GetCategoryByIdQuery { Id = categoryId }, ct);
            var hasParent = category?.ParentId != null;
            var backCallback = category?.ParentId == null ? "admin:products" : $"products:subcategories:{category.ParentId}";

            var keyboard = _keyboardFactory.CreateProductListKeyboard(
                pagedResult,
                categoryId,
                categoryId,
                hasParent,
                backCallback,
                MenuType.Admin);

            // Редактируем предыдущее сообщение, чтобы не плодить новые
            await _botClient.SendOrEditMessageAsync(
                chatId,
                query.Message,
                $"Товары в категории {category?.Name}:",
                replyMarkup: keyboard,
                ct: ct);
        }
    }
}
