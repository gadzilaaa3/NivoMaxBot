using NivoMaxBot.Messaging.Abstractions.Attachments.Inline;

namespace NivoMaxBot.Presentation.Common.Keyboards.Menu
{
    public interface IMenuBuilder
    {
        /// <summary>
        /// Добавляет управляющие кнопки (Назад, Меню) к существующей клавиатуре.
        /// </summary>
        /// <param name="existingButtons">Существующие ряды кнопок (например, список категорий)</param>
        /// <param name="backCallback">Callback для кнопки "Назад" (если null, кнопка не добавляется)</param>
        /// <param name="menuType">Тип меню (определяет callback для кнопки "Меню")</param>
        /// <returns>Новая клавиатура с добавленными кнопками</returns>
        IInlineKeyboardMarkup AddControlButtons(
            IEnumerable<IEnumerable<IInlineKeyboardButton>> existingButtons,
            string? backCallback,
            MenuType menuType);

        IInlineKeyboardMarkup CreateStartAdminMenu();

        IInlineKeyboardMarkup CreateMenu(MenuType menuType);
    }
}
