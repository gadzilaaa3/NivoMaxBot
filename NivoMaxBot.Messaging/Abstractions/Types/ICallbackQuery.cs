using System;
using System.Collections.Generic;
using System.Text;

namespace NivoMaxBot.Messaging.Abstractions.Types
{
    public interface ICallbackQuery
    {
        string? Id { get; }
        string? Data { get; }
        IMessage? Message { get; }
        IUser? From { get; }
    }
}
