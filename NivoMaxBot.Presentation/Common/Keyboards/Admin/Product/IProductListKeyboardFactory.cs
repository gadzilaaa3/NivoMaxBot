using NivoMaxBot.Application.Features.Products.Dtos;
using NivoMaxBot.Messaging.Abstractions.Attachments.Inline;
using NivoMaxBot.Presentation.Common.Keyboards.Menu;
using NivoMaxBot.Shared.Pagination;

namespace NivoMaxBot.Presentation.Common.Keyboards.Admin.Product
{
    public interface IProductListKeyboardFactory
    {
        IInlineKeyboardMarkup CreateProductListKeyboard(
            PagedResult<ProductDto> pagedResult,
            int categoryId,
            int? currentParentId, // для кнопки "Подкатегории" (если категория не является листом)
            bool hasParent,        // для кнопки "Назад"
            string backCallback,   // куда возвращаться
            MenuType menuType);
    }
}
