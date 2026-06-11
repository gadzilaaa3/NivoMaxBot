using MediatR;
using NivoMaxBot.Application.Features.Categories.Queries;
using NivoMaxBot.Application.Features.Categories.Queries.ById;
using NivoMaxBot.Application.Interfaces;
using NivoMaxBot.Messaging.Abstractions.Attachments.Inline;
using NivoMaxBot.Messaging.Abstractions.Types;

namespace NivoMaxBot.Presentation.Handlers.Callbacks.Products.Admin.Update.Categories
{
    public abstract class ProductEditCategoryHandlerBase
    {
        protected readonly IMediator _mediator;
        protected readonly IMessengerClient _botClient;
        protected readonly IUserStateService _userStateService;

        protected ProductEditCategoryHandlerBase(
            IMediator mediator, 
            IMessengerClient botClient, 
            IUserStateService userStateService)
        {
            _mediator = mediator;
            _botClient = botClient;
            _userStateService = userStateService;
        }

        protected async Task<InlineKeyboardMarkup> BuildCategoryTreeKeyboard(int? parentId, int productId, CancellationToken ct)
        {
            var categories = await _mediator.Send(new GetCategoriesQuery { ParentId = parentId }, ct);
            var buttons = new List<InlineKeyboardButton[]>();

            foreach (var cat in categories)
            {
                var text = cat.HasChildren ? $"📁 {cat.Name}" : $"📄 {cat.Name}";
                var callback = cat.HasChildren
                    ? $"product:edit:addcategory:list:{cat.Id}:{productId}"
                    : $"product:edit:addcategory:select:{cat.Id}:{productId}";
                buttons.Add(new[] { new InlineKeyboardButton(text, callback) });
            }

            if (parentId != null)
            {
                var parent = await _mediator.Send(new GetCategoryByIdQuery { Id = parentId.Value }, ct);
                var backCallback = parent?.ParentId == null
                    ? $"product:edit:addcategory:{productId}"
                    : $"product:edit:addcategory:list:{parent.ParentId}:{productId}";
                buttons.Add(new[] { new InlineKeyboardButton("🔙 Назад", backCallback) });
            }

            buttons.Add(new[] { new InlineKeyboardButton("❌ Отмена", $"product:edit:cancel:{productId}") });

            return new InlineKeyboardMarkup(buttons);
        }
    }
}
