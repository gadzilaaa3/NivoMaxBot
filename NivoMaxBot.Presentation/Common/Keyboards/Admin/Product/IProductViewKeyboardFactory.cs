using NivoMaxBot.Messaging.Abstractions.Attachments.Inline;
using NivoMaxBot.Presentation.Common.Keyboards.Menu;

namespace NivoMaxBot.Presentation.Common.Keyboards.Admin.Product
{
    public interface IProductViewKeyboardFactory
    {
        IInlineKeyboardMarkup CreateViewKeyboard(int productId, int? backCategoryId, MenuType menuType);
    }
}
