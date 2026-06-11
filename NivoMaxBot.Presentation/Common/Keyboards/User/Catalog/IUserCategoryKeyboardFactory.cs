using NivoMaxBot.Application.Features.Categories.Dtos;
using NivoMaxBot.Messaging.Abstractions.Attachments.Inline;

namespace NivoMaxBot.Presentation.Common.Keyboards.User.Catalog
{
    public interface IUserCategoryKeyboardFactory
    {
        IEnumerable<IEnumerable<IInlineKeyboardButton>> CreateCategoriesListButtons(
            IEnumerable<CategoryDto> categories);

        IInlineKeyboardMarkup CreateCategoriesListKeyboard(
            IEnumerable<CategoryDto> categories,
            string? backCallback);
    }
}
