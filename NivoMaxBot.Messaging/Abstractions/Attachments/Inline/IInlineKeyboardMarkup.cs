using System;
using System.Collections.Generic;
using System.Text;

namespace NivoMaxBot.Messaging.Abstractions.Attachments.Inline
{
    public interface IInlineKeyboardMarkup
    {
        IReadOnlyList<IReadOnlyList<IInlineKeyboardButton>> Buttons { get; }
    }
}
