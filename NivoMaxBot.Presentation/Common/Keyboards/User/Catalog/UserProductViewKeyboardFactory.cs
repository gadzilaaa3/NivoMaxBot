using NivoMaxBot.Messaging.Abstractions.Attachments.Inline;
using NivoMaxBot.Presentation.Common.Keyboards.Menu;

namespace NivoMaxBot.Presentation.Common.Keyboards.User.Catalog
{
    public class UserProductViewKeyboardFactory : IUserProductViewKeyboardFactory
    {
        private readonly IMenuBuilder _menuBuilder;

        public UserProductViewKeyboardFactory(IMenuBuilder menuBuilder)
        {
            _menuBuilder = menuBuilder;
        }

        public IInlineKeyboardMarkup CreateProductViewKeyboard(int productId, int? categoryId)
        {
            var buttons = new List<InlineKeyboardButton[]>
            {
                new[] { new InlineKeyboardButton("🛒 Добавить в корзину", $"user:cart:add:{productId}") }
            };

            string backCallback = categoryId.HasValue ? $"user:products:list:{categoryId}" : "user:catalog";
            return _menuBuilder.AddControlButtons(buttons, backCallback, MenuType.User);
        }
    }
}
