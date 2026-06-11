using NivoMaxBot.Messaging.Abstractions.Attachments.Inline;
using NivoMaxBot.Presentation.Common.Keyboards.Menu;

namespace NivoMaxBot.Presentation.Common.Keyboards.Admin.Product
{
    public class ProductViewKeyboardFactory : IProductViewKeyboardFactory
    {
        private readonly IMenuBuilder _menuBuilder;

        public ProductViewKeyboardFactory(IMenuBuilder menuBuilder)
        {
            _menuBuilder = menuBuilder;
        }

        public IInlineKeyboardMarkup CreateViewKeyboard(int productId, int? backCategoryId, MenuType menuType)
        {
            var buttons = new List<InlineKeyboardButton[]>
            {
                new[]
                {
                    new InlineKeyboardButton("📝 Редактировать", $"product:edit:{productId}"),
                    new InlineKeyboardButton("❌ Удалить", $"product:delete:{productId}")
                }
            };

            // Определяем callback для кнопки "Назад"
            string backCallback = backCategoryId.HasValue
                ? $"products:category:{backCategoryId}"
                : "admin:products"; // если категории нет, в корневой список товаров

            return _menuBuilder.AddControlButtons(buttons, backCallback, menuType);
        }
    }
}
