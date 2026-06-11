using NivoMaxBot.Application.Common;

namespace NivoMaxBot.Application.Interfaces
{
    public interface IUserStateService
    {
        UserState GetState(long userId);

        void SetState(long userId, UserState state);

        void ClearState(long userId);
    }
}
