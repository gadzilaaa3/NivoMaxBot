using NivoMaxBot.Application.Interfaces;
using NivoMaxBot.Messaging.Abstractions.Attachments.Inline;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.Routing;

namespace NivoMaxBot.Presentation.Handlers.Callbacks.Products.Admin.Delete
{
    [CallbackRoute("product:delete:{id:int}")]
    public class ProductDeleteHandler
    {
        private readonly IUserStateService _userStateService;
        private readonly IMessengerClient _botClient;

        public ProductDeleteHandler(IUserStateService userStateService, IMessengerClient botClient)
        {
            _userStateService = userStateService;
            _botClient = botClient;
        }

        public async Task HandleAsync(ICallbackQuery query, int id, CancellationToken ct)
        {
            var userId = query.From.Id;
            var keyboard = new InlineKeyboardMarkup(
            [
                [ 
                    new InlineKeyboardButton("✅ Да", $"product:delete_confirm:{id}"), 
                    new InlineKeyboardButton("❌ Нет", $"product:view:{id}") 
                ]
            ]);

            await _botClient.SendOrEditMessageAsync(query.Message.ChatId.Value, query.Message,
                "Вы уверены, что хотите удалить этот товар?", replyMarkup: keyboard, ct: ct);
        }
    }
}
