using Max.Bot.Types;
using NivoMaxBot.Messaging.Abstractions.Types;

namespace NivoMaxBot.MaxMessaging.Adapters
{
    public class MaxVideoAdapter : IVideo
    {
        private readonly VideoAttachment _video;
        public MaxVideoAdapter(VideoAttachment video)
        {
            _video = video;
        }

        public string FileId => _video.FileId;
        public int Width => _video.Width ?? 0;
        public int Height => _video.Height ?? 0;
        public int Duration => _video.Duration ?? 0;
    }
}
