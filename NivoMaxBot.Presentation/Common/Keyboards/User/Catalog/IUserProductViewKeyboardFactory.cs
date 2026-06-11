using NivoMaxBot.Messaging.Abstractions.Attachments.Inline;

namespace NivoMaxBot.Presentation.Common.Keyboards.User.Catalog
{
    public interface IUserProductViewKeyboardFactory
    {
        IInlineKeyboardMarkup CreateProductViewKeyboard(int productId, int? categoryId);
    }
}
