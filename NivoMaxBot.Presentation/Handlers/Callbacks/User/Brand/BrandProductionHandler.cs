using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.Routing;
using NivoMaxBot.Presentation.Common.Keyboards.Menu;
using NivoMaxBot.Presentation.Services.Brand;

namespace NivoMaxBot.Presentation.Handlers.Callbacks.User.Brand
{
    [CallbackRoute(UserBrandRoutes.Production)]
    public class BrandProductionHandler : BrandItemHandler
    {
        public BrandProductionHandler(
            IMessengerClient botClient,
            IBrandDataService brandDataService,
            IMenuBuilder menuBuilder)
            : base(botClient, brandDataService, menuBuilder)
        {
        }

        public async Task HandleAsync(ICallbackQuery query, CancellationToken ct)
        {
            var item = _brandDataService.Data.AboutProduction;
            await SendBrandItem(query.Message.ChatId.Value, item, ct);
        }
    }
}
