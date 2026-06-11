using MediatR;
using NivoMaxBot.Application.Interfaces;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.Routing;

namespace NivoMaxBot.Presentation.Handlers.Callbacks.Products.Admin.Update.Categories
{
    [CallbackRoute("product:edit:addcategory:{productId:int}")]
    public class ProductEditAddCategoryHandler : ProductEditCategoryHandlerBase
    {
        public ProductEditAddCategoryHandler(IMediator mediator, IMessengerClient botClient, IUserStateService userStateService)
            : base(mediator, botClient, userStateService) { }

        public async Task HandleAsync(ICallbackQuery query, int productId, CancellationToken ct)
        {
            var userId = query.From.Id;
            var state = _userStateService.GetState(userId);
            state.CurrentAction = "EditingProductAddCategory";
            state.EntityId = productId;
            state.Step = 1;
            _userStateService.SetState(userId, state);

            var keyboard = await BuildCategoryTreeKeyboard(null, productId, ct);
            await _botClient.SendTextMessageAsync(query.Message.ChatId.Value, 
                "Выберите категорию для добавления:", replyMarkup: keyboard, ct: ct);
        }
    }
}
