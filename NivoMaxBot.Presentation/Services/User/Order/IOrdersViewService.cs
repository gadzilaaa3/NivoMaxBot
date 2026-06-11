using NivoMaxBot.Messaging.Abstractions.Attachments.Inline;
using NivoMaxBot.Messaging.Abstractions.Types;

namespace NivoMaxBot.Presentation.Services.User.Order
{
    public interface IOrdersViewService
    {
        /// <summary>
        /// Строит текст и клавиатуру для списка заказов с пагинацией.
        /// </summary>
        Task<(string Text, IInlineKeyboardMarkup Keyboard)> BuildOrdersListAsync(int userId, int pageNumber, CancellationToken ct);

        /// <summary>
        /// Строит текст и клавиатуру для детального просмотра заказа.
        /// </summary>
        Task<(string Text, IInlineKeyboardMarkup Keyboard)> BuildOrderDetailsAsync(int orderId, int userId, CancellationToken ct);

        /// <summary>
        /// Отправляет сообщение со списком заказов.
        /// </summary>
        Task ShowOrdersListAsync(long chatId, IMessage message, 
            int userId, int pageNumber, CancellationToken ct);

        /// <summary>
        /// Отправляет сообщение с деталями заказа.
        /// </summary>
        Task ShowOrderDetailsAsync(long chatId, IMessage message, 
            int orderId, int userId, CancellationToken ct);
    }
}
