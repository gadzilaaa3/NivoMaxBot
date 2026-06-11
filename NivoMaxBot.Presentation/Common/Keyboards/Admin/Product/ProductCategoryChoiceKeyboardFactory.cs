using NivoMaxBot.Application.Features.Categories.Dtos;
using NivoMaxBot.Messaging.Abstractions.Attachments.Inline;
using NivoMaxBot.Presentation.Common.Keyboards.Menu;

namespace NivoMaxBot.Presentation.Common.Keyboards.Admin.Product
{
    public class ProductCategoryChoiceKeyboardFactory : IProductCategoryChoiceKeyboardFactory
    {
        private readonly IMenuBuilder _menuBuilder;

        public ProductCategoryChoiceKeyboardFactory(IMenuBuilder menuBuilder)
        {
            _menuBuilder = menuBuilder;
        }

        public IInlineKeyboardMarkup CreateCategoryChoiceKeyboard(
            IEnumerable<CategoryDto> categories,
            int? currentParentId,
            bool hasParent,
            string backCallback,
            MenuType menuType)
        {
            var buttons = new List<InlineKeyboardButton[]>();

            foreach (var cat in categories)
            {
                var text = cat.HasChildren ? $"📁 {cat.Name}" : $"📄 {cat.Name}";
                var callback = $"products:category:{cat.Id}";
                buttons.Add(new[] { new InlineKeyboardButton(text, callback) });
            }

            return _menuBuilder.AddControlButtons(buttons, backCallback, menuType);
        }
    }
}
