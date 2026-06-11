namespace NivoMaxBot.Messaging.Abstractions.Types
{
    public interface IPhoto
    {
        string FileId { get; }
        string? Url { get; }
        int Width { get; }
        int Height { get; }
    }
}
