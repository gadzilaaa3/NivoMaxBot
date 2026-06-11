namespace NivoMaxBot.Messaging.Abstractions.Attachments.Inline
{
    public class InlineKeyboardMarkup : IInlineKeyboardMarkup
    {
        public IReadOnlyList<IReadOnlyList<IInlineKeyboardButton>> Buttons { get; }

        public InlineKeyboardMarkup(IEnumerable<IEnumerable<IInlineKeyboardButton>> buttons)
        {
            Buttons = buttons
                .Select(row => row.ToList().AsReadOnly())
                .ToList()
                .AsReadOnly();
        }

        public InlineKeyboardMarkup(IReadOnlyList<IReadOnlyList<IInlineKeyboardButton>> buttons)
        {
            Buttons = buttons;
        }
    }
}
