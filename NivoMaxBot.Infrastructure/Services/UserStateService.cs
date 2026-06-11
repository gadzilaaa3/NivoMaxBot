using NivoMaxBot.Application.Common;
using NivoMaxBot.Application.Interfaces;
using System.Collections.Concurrent;

namespace NivoMaxBot.Infrastructure.Services
{
    public class UserStateService : IUserStateService
    {
        private readonly ConcurrentDictionary<long, UserState> _states = new();

        public UserState GetState(long userId)
        {
            return _states.GetOrAdd(userId, new UserState());
        }

        public void SetState(long userId, UserState state)
        {
            _states[userId] = state;
        }

        public void ClearState(long userId)
        {
            _states.TryRemove(userId, out _);
        }
    }
}
