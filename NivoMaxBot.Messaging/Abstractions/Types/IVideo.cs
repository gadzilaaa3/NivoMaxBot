namespace NivoMaxBot.Messaging.Abstractions.Types
{
    public interface IVideo
    {
        string FileId { get; }
        int Width { get; }
        int Height { get; }
        int Duration { get; }
    }
}
