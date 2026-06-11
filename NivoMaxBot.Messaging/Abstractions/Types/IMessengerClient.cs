using NivoMaxBot.Messaging.Abstractions.Attachments.Inline;
using NivoMaxBot.Messaging.Abstractions.Types.Enums;

namespace NivoMaxBot.Messaging.Abstractions.Types
{
    public interface IMessengerClient
    {
        Task<IMessage> SendTextMessageAsync(long chatId, string text,
            IInlineKeyboardMarkup? replyMarkup = null, TextFormat? textFormat = null, 
            CancellationToken ct = default);

        Task<IMessage> SendTextMessageToUserAsync(long userId, string text,
            IInlineKeyboardMarkup? replyMarkup = null, TextFormat? textFormat = null,
            CancellationToken ct = default);

        Task<IMessage> SendPhotoAsync(long chatId, InputFileStream photo,
            string? caption = null, IInlineKeyboardMarkup? replyMarkup = null,
            TextFormat? textFormat = null, CancellationToken ct = default);

        Task<IMessage> SendPhotoByFileIdAsync(long chatId, string fileId,
            string? caption = null, IInlineKeyboardMarkup? replyMarkup = null,
            TextFormat? textFormat = null, CancellationToken ct = default);

        Task<IMessage> SendPhotoByUrlAsync(long chatId, string url,
            string? caption = null, IInlineKeyboardMarkup? replyMarkup = null,
            TextFormat? textFormat = null, CancellationToken ct = default);

        Task<IMessage> SendVideoAsync(long chatId, InputFileStream video, 
            string? caption = null, IInlineKeyboardMarkup? replyMarkup = null, 
            TextFormat? textFormat = null, CancellationToken ct = default);

        Task AnswerCallbackQueryAsync(string callbackQueryId, string text = "", 
            CancellationToken ct = default);

        Task EditMessageTextAsync(long chatId, string messageId, 
            string text, IInlineKeyboardMarkup? replyMarkup = null, 
            TextFormat? textFormat = null, CancellationToken ct = default);

        Task<IMessage> CopyMessageAsync(long fromChatId, string messageId, long? chatId = null,
            long? userId = null, CancellationToken ct = default);

        Task SendOrEditMessageAsync(long chatId, IMessage? message, string text, 
            IInlineKeyboardMarkup? replyMarkup = null, 
            TextFormat? textFormat = null, CancellationToken ct = default);
    }
}
