using NivoMaxBot.Application.Interfaces;
using NivoMaxBot.Domain.Constants;
using NivoMaxBot.Messaging.Abstractions.Attachments.Inline;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Presentation.Common.Keyboards.Menu;
using NivoMaxBot.Presentation.Handlers.Callbacks.User;

namespace NivoMaxBot.Presentation.Services.MenuDisplay
{
    public class MenuDisplayService : IMenuDisplayService
    {
        private readonly IMenuPhotoService _photoService;
        private readonly IMenuBuilder _menuBuilder;
        private readonly IMessengerClient _botClient;

        public MenuDisplayService(
            IMenuPhotoService photoService, 
            IMenuBuilder menuBuilder,
            IMessengerClient messengerClient)
        {
            _photoService = photoService;
            _menuBuilder = menuBuilder;
            _botClient = messengerClient;
        }

        public async Task ShowUserMenu(long chatId, CancellationToken cancellationToken)
        {
            var keyboard = _menuBuilder.CreateMenu(MenuType.User);
            var photoPath = _photoService.GetPhotoPath(MenuSections.MainMenu);

            if (photoPath != null && File.Exists(photoPath))
            {
                using var stream = new FileStream(photoPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                await _botClient.SendPhotoAsync(chatId, new InputFileStream(stream), replyMarkup: keyboard, ct: cancellationToken);
            }
            else
            {
                await _botClient.SendTextMessageAsync(chatId, "Главное меню:", replyMarkup: keyboard, ct: cancellationToken);
            }
        }

        public async Task ShowAdminStartMenu(long chatId, CancellationToken cancellationToken)
        {
            var keyboard = _menuBuilder.CreateStartAdminMenu();
            var photoPath = _photoService.GetPhotoPath(MenuSections.MainMenu);

            if (photoPath != null && File.Exists(photoPath))
            {
                using var stream = new FileStream(photoPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                await _botClient.SendPhotoAsync(chatId,
                    new InputFileStream(stream), replyMarkup: keyboard, ct: cancellationToken);
            }
            else
            {
                await _botClient.SendTextMessageAsync(chatId, "Главное меню:", replyMarkup: keyboard, ct: cancellationToken);
            }
        }

        public async Task ShowAdminMenu(long chatId, CancellationToken cancellationToken)
        {
            var keyboard = _menuBuilder.CreateMenu(MenuType.Admin);
            var photoPath = _photoService.GetPhotoPath(MenuSections.MainMenu);

            if (photoPath != null && File.Exists(photoPath))
            {
                using var stream = new FileStream(photoPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                await _botClient.SendPhotoAsync(chatId,
                    new InputFileStream(stream), replyMarkup: keyboard, ct: cancellationToken);
            }
            else
            {
                await _botClient.SendTextMessageAsync(chatId, "Админ панель:", replyMarkup: keyboard, ct: cancellationToken);
            }
        }

        public async Task ShowCatalogRoot(long chatId, IEnumerable<IEnumerable<IInlineKeyboardButton>> existingButtons,
            CancellationToken cancellationToken)
        {
            var photoPath = _photoService.GetPhotoPath(MenuSections.CatalogRoot);

            var consultationButton = new InlineKeyboardButton(
                "📞 Получить консультацию", UserModeRoutes.ConsultationCreate);
            var buttons = existingButtons.ToList();
            buttons.Add([consultationButton]);
            var keyboard = _menuBuilder.AddControlButtons(buttons, null, MenuType.User);

            if (photoPath != null && File.Exists(photoPath))
            {
                using var stream = new FileStream(photoPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                await _botClient.SendPhotoAsync(chatId,
                    new InputFileStream(stream), replyMarkup: keyboard, ct: cancellationToken);
            }
            else
            {
                await _botClient.SendTextMessageAsync(chatId, "Каталог:", 
                    replyMarkup: keyboard, ct: cancellationToken);
            }
        }
    }
}
