using Max.Bot.Types;
using NivoMaxBot.Messaging.Abstractions.Types;

namespace NivoMaxBot.MaxMessaging.Adapters
{
    class MaxPhotoAdapter : IPhoto
    {
        private readonly PhotoAttachment _photo;
        public MaxPhotoAdapter(PhotoAttachment photo)
        {
            _photo = photo;
        }

        public string FileId => _photo.FileId;
        public int Width => _photo.Width;
        public int Height => _photo.Height;
        public string? Url => _photo.Url;
    }
}
