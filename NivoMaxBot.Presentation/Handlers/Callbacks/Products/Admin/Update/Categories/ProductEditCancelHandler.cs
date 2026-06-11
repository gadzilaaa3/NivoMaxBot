using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.Routing;

namespace NivoMaxBot.Presentation.Handlers.Callbacks.Products.Admin.Update.Categories
{
    [CallbackRoute("product:edit:cancel:{productId:int}")]
    public class ProductEditCancelHandler
    {
        private readonly IMessengerClient _botClient;

        public ProductEditCancelHandler(IMessengerClient botClient)
        {
            _botClient = botClient;
        }

        public async Task HandleAsync(ICallbackQuery query, int productId, CancellationToken ct)
        {
            // Просто возвращаем пользователя к диалогу редактирования
            await _botClient.SendTextMessageAsync(query.Message.ChatId.Value, 
                "Редактирование продолжается.", ct: ct);
        }
    }
}
