using MediatR;
using NivoMaxBot.Application.Features.Categories.Queries;
using NivoMaxBot.Application.Features.Categories.Queries.ById;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.Routing;
using NivoMaxBot.Presentation.Common.Keyboards.User.Catalog;

namespace NivoMaxBot.Presentation.Handlers.Callbacks.User.Catalog
{
    [CallbackRoute($"{UserCatalogRoutes.CategoryList}:{{parentId:int}}")]
    public class UserCategoryListHandler
    {
        private readonly IMediator _mediator;
        private readonly IUserCategoryKeyboardFactory _keyboardFactory;
        private readonly IMessengerClient _botClient;

        public UserCategoryListHandler(
            IMediator mediator,
            IUserCategoryKeyboardFactory keyboardFactory,
            IMessengerClient botClient)
        {
            _mediator = mediator;
            _keyboardFactory = keyboardFactory;
            _botClient = botClient;
        }

        public async Task HandleAsync(ICallbackQuery query, int parentId, CancellationToken ct)
        {
            var chatId = query.Message.ChatId.Value;
            var categories = await _mediator.Send(new GetCategoriesQuery { ParentId = parentId }, ct);
            var parentCategory = await _mediator.Send(new GetCategoryByIdQuery { Id = parentId }, ct);
           
            var backCallback = parentCategory?.ParentId == null 
                ? UserCatalogRoutes.CatalogRoot 
                : $"{UserCatalogRoutes.CategoryList}:{parentCategory.ParentId}";
            var keyboard = _keyboardFactory.CreateCategoriesListKeyboard(categories, backCallback);

            await _botClient.SendOrEditMessageAsync(chatId, query.Message, 
                "Выберите категорию:", keyboard, ct: ct);
        }
    }
}
