using System;
using System.Collections.Generic;
using System.Text;

namespace NivoMaxBot.Messaging.Abstractions.Types
{
    public interface IUser
    {
        long Id { get; }
        string? Username { get; }
        string? FirstName { get; }
        string? LastName { get; }
    }
}
