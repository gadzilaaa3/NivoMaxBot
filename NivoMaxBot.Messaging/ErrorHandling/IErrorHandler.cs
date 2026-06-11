using NivoMaxBot.Messaging.Abstractions.Attachments.Inline;

namespace NivoMaxBot.Messaging.ErrorHandling
{
    public interface IErrorHandler
    {
        Task HandleError(long chatId, Exception exception, CancellationToken ct);
        Task HandleError(long chatId, Exception exception, IInlineKeyboardMarkup? markup, CancellationToken ct);
    }
}
