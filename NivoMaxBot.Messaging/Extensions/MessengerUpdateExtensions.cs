using NivoMaxBot.Messaging.Abstractions.Types;

namespace NivoMaxBot.Messaging.Extensions
{
    public static class MessengerUpdateExtensions
    {
        public static long? GetUserId(this IMessengerUpdate update)
            => update.CallbackQuery?.From?.Id
                ?? update.Message?.From?.Id;
    }
}
