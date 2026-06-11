using NivoMaxBot.Application.Features.Categories.Dtos;
using NivoMaxBot.Messaging.Abstractions.Attachments.Inline;

namespace NivoMaxBot.Presentation.Common.Keyboards.Admin.Category.ParentSelection
{
    public interface ICategoryParentSelectionKeyboardBuilder
    {
        IInlineKeyboardMarkup BuildKeyboard(
            IEnumerable<CategoryDto> categories,
            int? currentParentId,
            int editId,
            int? backParentId);
    }
}
