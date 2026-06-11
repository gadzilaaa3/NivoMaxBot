using NivoMaxBot.Messaging.Abstractions.Attachments.Inline;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.Routing;

namespace NivoMaxBot.Presentation.Handlers.Callbacks.Categories.Admin.Delete
{
    [CallbackRoute($"{AdminCategoryRoutes.Delete}:{{id:int}}")]
    public class CategoryDeleteHandler
    {
        private readonly IMessengerClient _botClient;

        public CategoryDeleteHandler(IMessengerClient botClient) => _botClient = botClient;

        public async Task HandleAsync(ICallbackQuery query, int id, CancellationToken ct)
        {
            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    new InlineKeyboardButton("✅ Да", $"{AdminCategoryRoutes.DeleteConfirm}:{id}"),
                    new InlineKeyboardButton("❌ Нет", $"{AdminCategoryRoutes.View}:{id}")
                }
            });
            await _botClient.SendOrEditMessageAsync(query.Message.ChatId.Value, query.Message, 
                "Удалить категорию?", replyMarkup: keyboard, ct: ct);
        }
    }
}
