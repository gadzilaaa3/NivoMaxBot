using MediatR;
using NivoMaxBot.Application.Interfaces;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.Routing;

namespace NivoMaxBot.Presentation.Handlers.Callbacks.Products.Admin.Update.Categories
{
    [CallbackRoute("product:edit:addcategory:list:{parentId:int}:{productId:int}")]
    public class ProductEditAddCategoryListHandler : ProductEditCategoryHandlerBase
    {
        public ProductEditAddCategoryListHandler(
            IMediator mediator, 
            IMessengerClient botClient, 
            IUserStateService userStateService)
            : base(mediator, botClient, userStateService) { }

        public async Task HandleAsync(ICallbackQuery query, int parentId, int productId, CancellationToken ct)
        {
            var userId = query.From.Id;
            var state = _userStateService.GetState(userId);
            state.CurrentAction = "EditingProductAddCategory";
            state.EntityId = productId;
            state.Step = 1;
            _userStateService.SetState(userId, state);

            var keyboard = await BuildCategoryTreeKeyboard(parentId, productId, ct);
            await _botClient.SendOrEditMessageAsync(query.Message.ChatId.Value, 
                query.Message, "Выберите категорию:", replyMarkup: keyboard, ct: ct);
        }
    }
}
