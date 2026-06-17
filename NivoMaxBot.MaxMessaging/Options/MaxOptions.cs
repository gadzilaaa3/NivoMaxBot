namespace NivoMaxBot.MaxMessaging.Options
{
    public class MaxOptions
    {
        public const string SectionName = "MaxBot";
        public string Token { get; set; } = string.Empty;
        public string WebhookUrl { get; set; } = string.Empty;
        public string WebhookSecret { get; set; } = string.Empty;
    }
}
