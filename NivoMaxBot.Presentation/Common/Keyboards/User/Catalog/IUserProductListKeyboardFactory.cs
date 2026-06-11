using NivoMaxBot.Application.Features.Products.Dtos;
using NivoMaxBot.Messaging.Abstractions.Attachments.Inline;
using NivoMaxBot.Shared.Pagination;

namespace NivoMaxBot.Presentation.Common.Keyboards.User.Catalog
{
    public interface IUserProductListKeyboardFactory
    {
        IInlineKeyboardMarkup CreateProductListKeyboard(
            PagedResult<ProductDto> pagedResult,
            int categoryId,
            string? backCallback);
    }
}
