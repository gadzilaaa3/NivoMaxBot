using MediatR;
using NivoMaxBot.Application.Features.Categories.Queries;
using NivoMaxBot.Application.Features.Categories.Queries.ById;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.Routing;
using NivoMaxBot.Presentation.Common.Keyboards.Admin.Category;

namespace NivoMaxBot.Presentation.Handlers.Callbacks.Categories.Admin.View
{
    [CallbackRoute(AdminCategoryRoutes.List)]
    [CallbackRoute($"{AdminCategoryRoutes.ParentChildrenList}:{{parentId:int?}}")]
    public class CategoryListHandler
    {
        private readonly IMediator _mediator;
        private readonly IMessengerClient _botClient;
        private readonly IAdminCategoryKeyboardFactory _keyboardFactory;

        public CategoryListHandler(
            IMediator mediator, 
            IMessengerClient botClient, 
            IAdminCategoryKeyboardFactory keyboardFactory)
        {
            _mediator = mediator;
            _botClient = botClient;
            _keyboardFactory = keyboardFactory;
        }

        public async Task HandleAsync(ICallbackQuery query, int? parentId, CancellationToken ct)
        {
            var chatId = query.Message.ChatId.Value;
            var categories = await _mediator.Send(new GetCategoriesQuery { ParentId = parentId }, ct);

            // Вычисляем callback для кнопки "Назад"
            string? backCallback = null;
            if (parentId.HasValue)
            {
                var parentCategory = await _mediator.Send(new GetCategoryByIdQuery { Id = parentId.Value }, ct);
                //backCallback = parentCategory?.ParentId == null ? $"{AdminCategoryRoutes.List}" :
                //    $"{AdminCategoryRoutes.ParentChildrenList}:{parentCategory.ParentId}";
                backCallback = parentId == null ? $"{AdminCategoryRoutes.List}" :
                    $"{AdminCategoryRoutes.View}:{parentId}";
            }

            var keyboard = _keyboardFactory.CreateCategoriesListKeyboard(categories, parentId, backCallback);
            var text = parentId == null ? "Корневые категории:" : "Подкатегории:";
            await _botClient.SendOrEditMessageAsync(chatId, query.Message,
                text, replyMarkup: keyboard, ct: ct);
        }
    }
}
