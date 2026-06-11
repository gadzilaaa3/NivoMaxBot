namespace NivoMaxBot.Messaging.Abstractions.Types
{
    public interface IMessengerUpdateDispatcher
    {
        Task HandleAsync(IMessengerUpdate update, CancellationToken cancellationToken);
    }
}
