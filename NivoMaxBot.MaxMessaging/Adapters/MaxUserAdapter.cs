using Max.Bot.Types;
using NivoMaxBot.Messaging.Abstractions.Types;

namespace NivoMaxBot.MaxMessaging.Adapters
{
    public class MaxUserAdapter : IUser
    {
        private readonly User _user;
        public MaxUserAdapter(User user) => _user = user;
        public long Id => _user.Id;
        public string? Username => _user.Username;
        public string? FirstName => _user.FirstName;
        public string? LastName => _user.LastName;
    }
}
