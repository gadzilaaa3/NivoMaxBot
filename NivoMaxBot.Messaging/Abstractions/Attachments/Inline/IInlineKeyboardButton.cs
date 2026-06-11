using System;
using System.Collections.Generic;
using System.Text;

namespace NivoMaxBot.Messaging.Abstractions.Attachments.Inline
{
    public interface IInlineKeyboardButton
    {
        string Text { get; }
        string? CallbackData { get; }
        string? Url { get; }
    }
}
