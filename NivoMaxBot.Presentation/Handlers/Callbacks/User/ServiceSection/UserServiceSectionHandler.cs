using NivoMaxBot.Messaging.Abstractions.Attachments.Inline;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.Routing;
using NivoMaxBot.Presentation.Common.Keyboards.Menu;

namespace NivoMaxBot.Presentation.Handlers.Callbacks.User.ServiceSection
{
    [CallbackRoute(UserModeRoutes.ServiceSection)]
    public class UserServiceSectionHandler
    {
        private readonly IMessengerClient _botClient;
        private readonly IMenuBuilder _menuBuilder;
        public UserServiceSectionHandler(
            IMessengerClient botClient,
            IMenuBuilder menuBuilder)
        {
            _botClient = botClient;
            _menuBuilder = menuBuilder;
        }
        public async Task HandleAsync(ICallbackQuery query, CancellationToken ct)
        {
            IEnumerable<IEnumerable<InlineKeyboardButton>> buttons =
            [
                [new InlineKeyboardButton("🛠️ Сервисная служба", UserModeRoutes.ServiceDepartment)],
                [new InlineKeyboardButton("🔧 Оставить заявку на гарантию", "user:warranty_request:create")],
            ];

            var keyboard = _menuBuilder.AddControlButtons(buttons, null, MenuType.User);
            await _botClient.SendOrEditMessageAsync(query.Message.ChatId.Value, query.Message, 
                "Сервис и гарантия:", replyMarkup: keyboard, ct: ct);
        }
    }
}
