using System;
using System.Collections.Generic;
using System.Text;

namespace NivoMaxBot.Messaging.Abstractions.Types
{
    public interface IMessengerUpdate
    {
        IMessage? Message { get; }
        ICallbackQuery? CallbackQuery { get; }
    }
}
