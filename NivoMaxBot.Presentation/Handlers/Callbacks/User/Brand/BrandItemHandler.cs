using NivoMaxBot.Messaging.Abstractions.Attachments.Inline;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Presentation.Common.Keyboards.Menu;
using NivoMaxBot.Presentation.Services.Brand;
using NivoMaxBot.Shared.Brand;
using System.Reflection;

namespace NivoMaxBot.Presentation.Handlers.Callbacks.User.Brand
{
    public abstract class BrandItemHandler
    {
        protected readonly IMessengerClient _botClient;
        protected readonly IBrandDataService _brandDataService;
        protected readonly IMenuBuilder _menuBuilder;

        protected BrandItemHandler(
            IMessengerClient botClient,
            IBrandDataService brandDataService,
            IMenuBuilder menuBuilder)
        {
            _botClient = botClient;
            _brandDataService = brandDataService;
            _menuBuilder = menuBuilder;
        }

        protected async Task SendBrandItem(long chatId, 
            BrandItem item, CancellationToken ct)
        {
            var keyboard = _menuBuilder.AddControlButtons([], UserBrandRoutes.Menu, MenuType.User);

            if (!string.IsNullOrEmpty(item.VideoFileName))
            {
                await SendBrandVideoAsync(chatId, item.VideoFileName, keyboard, item.Text, ct);
            }
            else
            {
                await _botClient.SendTextMessageAsync(chatId, item.Text, 
                    replyMarkup: keyboard, ct: ct);
            }
        }

        protected async Task SendBrandVideoAsync(long chatId, string fileName, 
            IInlineKeyboardMarkup keyboard, 
            string caption, CancellationToken ct)
        {
            // var basePath = AppContext.BaseDirectory;
            var basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var filePath = Path.Combine(basePath, "Resources", fileName);
            if (!File.Exists(filePath))
            {
                // Если видео отсутствует, отправляем только текст
                await _botClient.SendTextMessageAsync(chatId, caption, 
                    replyMarkup: keyboard, ct: ct);
                return;
            }
            using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            await _botClient.SendVideoAsync(chatId, new InputFileStream(stream), 
                replyMarkup:keyboard, caption: caption, ct: ct);
        }
    }
}
