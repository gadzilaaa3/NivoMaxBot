using NivoMaxBot.Application.Interfaces;

namespace NivoMaxBot.Infrastructure.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private long? _maxId;
        public long? MaxId => _maxId;
        public bool IsAuthenticated => _maxId.HasValue;

        public void SetUser(long maxId) => _maxId = maxId;
        public void Clear() => _maxId = null;
    }
}
