using NivoMaxBot.Application.Features.Categories.Dtos;
using NivoMaxBot.Messaging.Abstractions.Attachments.Inline;

namespace NivoMaxBot.Presentation.Common.Keyboards.Admin.Category
{
    public interface IAdminCategoryKeyboardFactory
    {
        /// <summary>
        /// Создаёт клавиатуру для списка категорий (админка).
        /// </summary>
        /// <param name="categories">Список категорий</param>
        /// <param name="currentParentId">Текущий родитель (null для корня)</param>
        /// <param name="backCallback">Callback для кнопки "Назад" (если null, кнопка не добавляется)</param>
        /// <returns>Инлайн-клавиатура</returns>
        IInlineKeyboardMarkup CreateCategoriesListKeyboard(
            IEnumerable<CategoryDto> categories,
            int? currentParentId,
            string? backCallback);

        /// <summary>
        /// Создаёт клавиатуру для просмотра одной категории (админка).
        /// </summary>
        /// <param name="categoryId">ID категории</param>
        /// <param name="parentId">ID родителя (для кнопки назад)</param>
        IInlineKeyboardMarkup CreateCategoryViewKeyboard(int categoryId, int? parentId);
    }
}
