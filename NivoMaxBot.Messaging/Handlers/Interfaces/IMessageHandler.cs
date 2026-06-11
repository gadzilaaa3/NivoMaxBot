using NivoMaxBot.Messaging.Abstractions.Types;

namespace NivoMaxBot.Messaging.Handlers.Interfaces
{
    public interface IMessageHandler
    {
        Task HandleAsync(IMessage message, CancellationToken cancellationToken);

        bool CanHandle(IMessage message);
    }
}
