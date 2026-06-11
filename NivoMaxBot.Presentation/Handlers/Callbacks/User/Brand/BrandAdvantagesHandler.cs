using NivoMaxBot.Messaging.Abstractions.Attachments.Inline;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.Routing;
using NivoMaxBot.Presentation.Common.Keyboards.Menu;
using NivoMaxBot.Presentation.Services.Brand;

namespace NivoMaxBot.Presentation.Handlers.Callbacks.User.Brand
{
    [CallbackRoute(UserBrandRoutes.Advantages)]
    public class BrandAdvantagesHandler
    {
        private readonly IBrandDataService _brandDataService;
        private readonly IMessengerClient _botClient;
        private readonly IMenuBuilder _menuBuilder;

        public BrandAdvantagesHandler(
            IBrandDataService brandDataService,
            IMessengerClient botClient,
            IMenuBuilder menuBuilder)
        {
            _botClient = botClient;
            _brandDataService = brandDataService;
            _menuBuilder = menuBuilder;
        }

        public async Task HandleAsync(ICallbackQuery query, CancellationToken ct)
        {
            var data = _brandDataService.Data.Advantages;
            var buttons = new List<InlineKeyboardButton[]>();

            for (int i = 0; i < data.Items.Count(); i++)
            {
                var item = data.Items.ElementAt(i);
                buttons.Add([ new InlineKeyboardButton(item.Header, 
                    $"{UserBrandRoutes.AdvantageDetail}:{i}") ]);
            }
            var keyboard = _menuBuilder.AddControlButtons(buttons, UserBrandRoutes.Menu, MenuType.User);

            await _botClient.SendOrEditMessageAsync(query.Message.ChatId.Value, query.Message,
                data.Title, replyMarkup: keyboard, ct: ct);
        }
    }
}
