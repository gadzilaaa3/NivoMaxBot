using Max.Bot.Types;
using NivoMaxBot.Messaging.Abstractions.Types;

namespace NivoMaxBot.MaxMessaging.Adapters
{
    public class MaxUpdateAdapter : IMessengerUpdate
    {
        private readonly Update _update;
        private IMessage? _message;

        public MaxUpdateAdapter(Update update)
        {
            _update = update;
            if (_update.Message != null)
                _message = new MaxMessageAdapter(_update.Message);
        }

        public IMessage? Message => _message;

        public ICallbackQuery? CallbackQuery => _update.Callback != null
            ? new MaxCallbackAdapter(_update.Callback, _message) : null;
    }
}
