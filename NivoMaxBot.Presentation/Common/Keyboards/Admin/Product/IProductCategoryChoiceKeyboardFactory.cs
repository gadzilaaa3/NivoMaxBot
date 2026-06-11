using NivoMaxBot.Application.Features.Categories.Dtos;
using NivoMaxBot.Messaging.Abstractions.Attachments.Inline;
using NivoMaxBot.Presentation.Common.Keyboards.Menu;

namespace NivoMaxBot.Presentation.Common.Keyboards.Admin.Product
{
    public interface IProductCategoryChoiceKeyboardFactory
    {
        IInlineKeyboardMarkup CreateCategoryChoiceKeyboard(
            IEnumerable<CategoryDto> categories,
            int? currentParentId,
            bool hasParent,
            string backCallback,
            MenuType menuType);
    }
}
