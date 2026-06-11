namespace NivoMaxBot.Application.Features.Broadcast.Dtos
{
    public class BroadcastResult
    {
        public int TotalUsers { get; set; }

        public int SuccessCount;

        public int FailedCount;

        public List<string> Errors { get; set; } = new();
    }
}
