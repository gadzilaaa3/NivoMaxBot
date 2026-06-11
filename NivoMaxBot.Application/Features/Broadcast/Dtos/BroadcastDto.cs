namespace NivoMaxBot.Application.Features.Broadcast.Dtos
{
    public class BroadcastDto
    {
        public long ToUserId { get; set; }

        public long FromChatId { get; set; }

        public string MessageId { get; set; } = string.Empty;
    }
}
