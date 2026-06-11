using Max.Bot.Types;
using NivoMaxBot.Messaging.Abstractions.Types;

namespace NivoMaxBot.MaxMessaging.Adapters
{
    public class MaxCallbackAdapter : ICallbackQuery
    {
        private readonly CallbackQuery _callback;
        private readonly IMessage? _message;
        public MaxCallbackAdapter(CallbackQuery callback, IMessage? message)
        {
            _callback = callback;
            _message = message;
        }
        public string? Id => _callback.CallbackId;
        public string? Data => _callback.Payload;

        public IMessage? Message => _message;
        public IUser? From => _callback.User != null ? new MaxUserAdapter(_callback.User) : null;
    }
}
