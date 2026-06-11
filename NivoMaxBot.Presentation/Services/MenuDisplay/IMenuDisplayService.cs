using NivoMaxBot.Messaging.Abstractions.Attachments.Inline;

namespace NivoMaxBot.Presentation.Services.MenuDisplay
{
    public interface IMenuDisplayService
    {
        Task ShowUserMenu(long chatId, CancellationToken cancellationToken);

        Task ShowAdminStartMenu(long chatId, CancellationToken cancellationToken);

        Task ShowAdminMenu(long chatId, CancellationToken cancellationToken);

        Task ShowCatalogRoot(long chatId, IEnumerable<IEnumerable<IInlineKeyboardButton>> buttons, CancellationToken cancellationToken);
    }
}
