namespace NivoMaxBot.Messaging.Abstractions.Types
{
    public interface IMessage
    {
        string? MessageId { get; }
        long? ChatId { get; }
        IUser? From { get; }
        string? Text { get; }
        IPhoto? Photo { get; }
        IVideo? Video { get; }
    }
}
