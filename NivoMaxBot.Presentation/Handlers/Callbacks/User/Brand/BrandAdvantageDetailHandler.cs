using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.Abstractions.Types.Enums;
using NivoMaxBot.Messaging.Routing;
using NivoMaxBot.Presentation.Common.Keyboards.Menu;
using NivoMaxBot.Presentation.Services.Brand;
using System.Reflection;

namespace NivoMaxBot.Presentation.Handlers.Callbacks.User.Brand
{
    [CallbackRoute($"{UserBrandRoutes.AdvantageDetail}:{{index:int}}")]
    public class BrandAdvantageDetailHandler
    {
        private readonly IBrandDataService _brandData;
        private readonly IMessengerClient _botClient;
        private readonly IMenuBuilder _menuBuilder;

        public BrandAdvantageDetailHandler(
            IBrandDataService brandData, 
            IMessengerClient botClient,
            IMenuBuilder menuBuilder)
        {
            _brandData = brandData;
            _botClient = botClient;
            _menuBuilder = menuBuilder;
        }

        public async Task HandleAsync(ICallbackQuery query, int index, CancellationToken ct)
        {
            var advantages = _brandData.Data.Advantages.Items;
            if (index < 0 || index >= advantages.Count())
            {
                return;
            }

            var item = advantages.ElementAt(index);
            var text = $"*{item.Header}*\n\n{item.Description}";
            var basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var photoPath = Path.Combine(basePath, "Resources", "Advantages", item.PhotoFileName);

            var keyboard = _menuBuilder.AddControlButtons([], UserBrandRoutes.Advantages, MenuType.User);

            if (File.Exists(photoPath))
            {
                using var stream = new FileStream(photoPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                await _botClient.SendPhotoAsync(
                    query.Message.ChatId.Value,
                    new InputFileStream(stream),
                    replyMarkup: keyboard,
                    caption: text,
                    textFormat: TextFormat.Markdown,
                    ct: ct);
            }
            else
            {
                // Если фото нет, отправляем только текст
                await _botClient.SendOrEditMessageAsync(query.Message.ChatId.Value, query.Message,
                    text, textFormat: TextFormat.Markdown, 
                    replyMarkup: keyboard, ct: ct);
            }
        }
    }
}
