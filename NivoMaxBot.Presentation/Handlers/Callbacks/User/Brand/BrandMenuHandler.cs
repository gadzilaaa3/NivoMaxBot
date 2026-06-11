using NivoMaxBot.Messaging.Abstractions.Attachments.Inline;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.Routing;
using NivoMaxBot.Presentation.Common.Keyboards.Menu;
using NivoMaxBot.Presentation.Services.Brand;

namespace NivoMaxBot.Presentation.Handlers.Callbacks.User.Brand
{
    [CallbackRoute(UserBrandRoutes.Menu)]
    public class BrandMenuHandler
    {
        private readonly IMessengerClient _botClient;
        private readonly IMenuBuilder _menuBuilder;
        private readonly IBrandDataService _brandDataService;

        public BrandMenuHandler(
            IMessengerClient botClient,
            IMenuBuilder menuBuilder,
            IBrandDataService brandDataService)
        {
            _botClient = botClient;
            _menuBuilder = menuBuilder;
            _brandDataService = brandDataService;
        }
        public async Task HandleAsync(ICallbackQuery query, CancellationToken ct)
        {
            IEnumerable<IEnumerable<InlineKeyboardButton>> buttons = 
            [
                [
                    new InlineKeyboardButton("🏢 О бренде", UserBrandRoutes.About),
                    new InlineKeyboardButton("🏭 О производстве", UserBrandRoutes.Production)
                ],
                [
                    new InlineKeyboardButton("⭐ Наши преимущества", UserBrandRoutes.Advantages)
                ],
                [
                    new InlineKeyboardButton("📦 Дилеры", url: _brandDataService.Data.DealersUrl),
                    new InlineKeyboardButton("🌐 Сайт", url: _brandDataService.Data.WebsiteUrl),
                ]
            ];

            var keyboard = _menuBuilder.AddControlButtons(buttons, null, MenuType.User);

            await _botClient.SendOrEditMessageAsync(query.Message.ChatId.Value, query.Message,
                "Информация о бренде:", replyMarkup: keyboard, ct: ct);
        }
    }
}
