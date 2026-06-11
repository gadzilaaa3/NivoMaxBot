namespace NivoMaxBot.Messaging.Abstractions.Attachments.Inline
{
    public class InlineKeyboardButton : IInlineKeyboardButton
    {
        public string Text { get; }
        public string? CallbackData { get; }
        public string? Url { get; }

        public InlineKeyboardButton(string text, string? callbackData = null, string? url = null)
        {
            Text = text;
            CallbackData = callbackData;
            Url = url;
        }
    }
}
