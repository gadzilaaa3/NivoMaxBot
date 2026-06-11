using Max.Bot.Types;
using NivoMaxBot.Messaging.Abstractions.Types;

namespace NivoMaxBot.MaxMessaging.Adapters
{
    public class MaxMessageAdapter : IMessage
    {
        private readonly Message _maxMessage;

        public MaxMessageAdapter(Message maxMessage)
        {
            _maxMessage = maxMessage;
        }

        public string? MessageId => _maxMessage.Mid;
        public long? ChatId => _maxMessage?.Recipient?.ChatId;
        public IUser? From => _maxMessage.Sender != null ? new MaxUserAdapter(_maxMessage.Sender) : null;
        public string? Text => _maxMessage.Text;

        public IPhoto? Photo
        {
            get
            {
                var photo = _maxMessage?.Body?.Attachments
                    .FirstOrDefault(a => a is PhotoAttachment) as PhotoAttachment;

                return photo != null ? new MaxPhotoAdapter(photo) : null;
            }
        }

        public IVideo? Video
        {
            get
            {
                var video = _maxMessage?.Body?.Attachments
                    .FirstOrDefault(a => a is VideoAttachment) as VideoAttachment;

                return video != null ? new MaxVideoAdapter(video) : null;
            }
        }
    }
}
