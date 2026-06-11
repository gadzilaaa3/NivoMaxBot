using MediatR;
using NivoMaxBot.Application.Features.Categories.Queries;
using NivoMaxBot.Application.Features.Categories.Queries.ById;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.Routing;
using NivoMaxBot.Presentation.Common.Keyboards.Admin.Category.ParentSelection;

namespace NivoMaxBot.Presentation.Handlers.Callbacks.Categories.Admin.Edit
{
    [CallbackRoute($"{AdminCategoryRoutes.ParentSelection}:{{parentId:int}}:for:{{editId:int}}")]
    public class CategoryParentSelectionHandler
    {
        private readonly IMediator _mediator;
        private readonly IMessengerClient _botClient;
        private readonly ICategoryParentSelectionKeyboardBuilder _keyboardBuilder;

        public CategoryParentSelectionHandler(
            IMediator mediator,
            IMessengerClient botClient,
            ICategoryParentSelectionKeyboardBuilder keyboardBuilder)
        {
            _mediator = mediator;
            _botClient = botClient;
            _keyboardBuilder = keyboardBuilder;
        }

        public async Task HandleAsync(ICallbackQuery query, int parentId, int editId, CancellationToken ct)
        {
            int? id = parentId;
            if (id == 0)
                id = null;
            await ShowParentCategoriesAsync(query, id, editId, ct);
        }

        private async Task ShowParentCategoriesAsync(
            ICallbackQuery query,
            int? parentId,
            int editId,
            CancellationToken ct)
        {
            var categories = await _mediator.Send(new GetCategoriesQuery { ParentId = parentId }, ct);
            int? backParentId = null;
            if (parentId.HasValue)
            {
                var parentCategory = await _mediator.Send(new GetCategoryByIdQuery { Id = parentId.Value }, ct);
                backParentId = parentCategory?.ParentId;
            }

            var keyboard = _keyboardBuilder.BuildKeyboard(categories, parentId, editId, backParentId);
            var text = parentId == null ? "Корневые категории (выберите родителя):" : "Подкатегории:";
            await _botClient.SendOrEditMessageAsync(
                query.Message.ChatId.Value,
                query.Message,
                text,
                replyMarkup: keyboard,
                ct: ct);
        }
    }
}
