using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.Routing;
using NivoMaxBot.Presentation.Common.Keyboards.Menu;
using NivoMaxBot.Presentation.Services.Brand;

namespace NivoMaxBot.Presentation.Handlers.Callbacks.User.Brand
{
    [CallbackRoute(UserBrandRoutes.About)]
    public class BrandAboutHandler : BrandItemHandler
    {
        public BrandAboutHandler(
            IMessengerClient botClient, 
            IBrandDataService brandDataService,
            IMenuBuilder menuBuilder) 
            : base(botClient, brandDataService, menuBuilder)
        {
        }

        public async Task HandleAsync(ICallbackQuery query, CancellationToken ct)
        {
            var item = _brandDataService.Data.AboutBrand;
            await SendBrandItem(query.Message.ChatId.Value, item, ct);
        }
    }
}
