namespace NivoMaxBot.Messaging.Abstractions.Types
{
    public class InputFileStream : IDisposable
    {
        public Stream Stream { get; }
        public string? FileName { get; }
        public InputFileStream(Stream stream, string? fileName = null)
        {
            Stream = stream;
            FileName = fileName;
        }
        public void Dispose() => Stream.Dispose();
    }
}
