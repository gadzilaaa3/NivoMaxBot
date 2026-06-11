using NivoMaxBot.Messaging.Abstractions.Attachments.Inline;
using NivoMaxBot.Messaging.Abstractions.Types;

namespace NivoMaxBot.Presentation.Services.User.WarrantyRequest
{
    public interface IWarrantyRequestsViewService
    {
        Task<(string Text, IInlineKeyboardMarkup Keyboard)> BuildRequestsListAsync(int userId, int pageNumber, CancellationToken ct);
        Task<(string Text, IInlineKeyboardMarkup Keyboard)> BuildRequestDetailsAsync(int requestId, int userId, CancellationToken ct);
        Task ShowRequestsListAsync(long chatId, IMessage? message,
            int userId, int pageNumber, CancellationToken ct);
        Task ShowRequestDetailsAsync(long chatId, IMessage? message,
            int requestId, int userId, CancellationToken ct);
    }
}
