using NivoMaxBot.Application.Features.Categories.Dtos;
using NivoMaxBot.Messaging.Abstractions.Attachments.Inline;
using NivoMaxBot.Presentation.Common.Keyboards.Menu;

namespace NivoMaxBot.Presentation.Common.Keyboards.User.Catalog
{
    public class UserCategoryKeyboardFactory : IUserCategoryKeyboardFactory
    {
        private readonly IMenuBuilder _menuBuilder;

        public UserCategoryKeyboardFactory(IMenuBuilder menuBuilder)
        {
            _menuBuilder = menuBuilder;
        }

        public IEnumerable<IEnumerable<IInlineKeyboardButton>> CreateCategoriesListButtons(
            IEnumerable<CategoryDto> categories)
        {
            var buttons = categories.Select(cat =>
            {
                var text = cat.HasChildren ? $"📁 {cat.Name}" : $"📄 {cat.Name}";
                var callback = cat.HasChildren ? $"user:category:list:{cat.Id}" : $"user:products:list:{cat.Id}";
                return new[] { new InlineKeyboardButton(text, callback) };
            }).ToList();

            return buttons;
        }

        public IInlineKeyboardMarkup CreateCategoriesListKeyboard(
            IEnumerable<CategoryDto> categories, string? backCallback)
        {
            var buttons = CreateCategoriesListButtons(categories);

            return _menuBuilder.AddControlButtons(buttons, backCallback, MenuType.User);
        }
    }
}
