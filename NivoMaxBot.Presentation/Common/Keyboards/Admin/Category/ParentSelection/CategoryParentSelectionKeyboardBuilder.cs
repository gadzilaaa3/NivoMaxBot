using NivoMaxBot.Application.Features.Categories.Dtos;
using NivoMaxBot.Messaging.Abstractions.Attachments.Inline;
using NivoMaxBot.Presentation.Handlers.Callbacks.Categories.Admin;

namespace NivoMaxBot.Presentation.Common.Keyboards.Admin.Category.ParentSelection
{
    public class CategoryParentSelectionKeyboardBuilder : ICategoryParentSelectionKeyboardBuilder
    {
        public IInlineKeyboardMarkup BuildKeyboard(
            IEnumerable<CategoryDto> categories,
            int? currentParentId,
            int editId,
            int? backParentId)
        {
            var buttons = new List<InlineKeyboardButton[]>();

            foreach (var cat in categories)
            {
                if (cat.HasChildren)
                {
                    buttons.Add(new[] { new InlineKeyboardButton($"📁 {cat.Name}",
                    $"{AdminCategoryRoutes.ParentSelection}:{cat.Id}:for:{editId}") });
                }
                else
                {
                    buttons.Add(new[] { new InlineKeyboardButton($"📄 {cat.Name}",
                    $"{AdminCategoryRoutes.SelectParent}:{cat.Id}:for:{editId}") });
                }
            }

            // Кнопка "Корень"
            buttons.Add(new[] { new InlineKeyboardButton("🌐 Выбрать",
            $"{AdminCategoryRoutes.SelectParent}:{currentParentId ?? 0}:for:{editId}") });

            // Кнопка "Назад"
            if (backParentId == null)
                buttons.Add(new[] { new InlineKeyboardButton("🔙 Назад",
                $"{AdminCategoryRoutes.ParentSelection}:{editId}") });
            else
                buttons.Add(new[] { new InlineKeyboardButton("🔙 Назад",
                $"{AdminCategoryRoutes.ParentSelection}:{backParentId}:for:{editId}") });

            return new InlineKeyboardMarkup(buttons);
        }
    }
}
