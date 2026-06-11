using NivoMaxBot.Application.Features.Categories.Dtos;
using NivoMaxBot.Messaging.Abstractions.Attachments.Inline;
using NivoMaxBot.Presentation.Common.Keyboards.Menu;
using NivoMaxBot.Presentation.Handlers.Callbacks.Categories.Admin;

namespace NivoMaxBot.Presentation.Common.Keyboards.Admin.Category
{
    public class AdminCategoryKeyboardFactory : IAdminCategoryKeyboardFactory
    {
        private readonly IMenuBuilder _menuBuilder;

        public AdminCategoryKeyboardFactory(IMenuBuilder menuBuilder)
        {
            _menuBuilder = menuBuilder;
        }

        public IInlineKeyboardMarkup CreateCategoriesListKeyboard(
            IEnumerable<CategoryDto> categories,
            int? currentParentId,
            string? backCallback)
        {
            var buttons = new List<InlineKeyboardButton[]>();

            // Кнопки самих категорий
            foreach (var cat in categories)
            {
                var text = cat.HasChildren ? $"📁 {cat.Name}" : $"📄 {cat.Name}";
                buttons.Add([new InlineKeyboardButton(text, $"{AdminCategoryRoutes.View}:{cat.Id}")]);
            }

            // Кнопка добавления категории
            var addCallback = currentParentId == null ? $"{AdminCategoryRoutes.Add}" 
                : $"{AdminCategoryRoutes.Add}:{currentParentId}";
            buttons.Add([new InlineKeyboardButton("➕ Добавить", addCallback)]);

            // Добавляем общие управляющие кнопки (Назад, Меню)
            return _menuBuilder.AddControlButtons(buttons, backCallback, MenuType.Admin);
        }

        public IInlineKeyboardMarkup CreateCategoryViewKeyboard(int categoryId, int? parentId)
        {
            var buttons = new List<InlineKeyboardButton[]>();
            buttons.AddRange([
                [new InlineKeyboardButton("📝 Редактировать", $"{AdminCategoryRoutes.Edit}:{categoryId}")],
                [new InlineKeyboardButton("❌ Удалить", $"{AdminCategoryRoutes.Delete}:{categoryId}")],
                [new InlineKeyboardButton("📂 Подкатегории", $"{AdminCategoryRoutes.ParentChildrenList}:{categoryId}")]
            ]);

            // Определяем callback для кнопки "Назад"
            var backCallback = parentId == null ? $"{AdminCategoryRoutes.List}" 
                : $"{AdminCategoryRoutes.ParentChildrenList}:{parentId}";

            return _menuBuilder.AddControlButtons(buttons, backCallback, MenuType.Admin);
        }
    }
}
