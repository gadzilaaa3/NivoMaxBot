using NivoMaxBot.Application.Features.Users.Dtos;
using NivoMaxBot.Messaging.Abstractions.Types;

namespace NivoMaxBot.Presentation.Services.User.Profile
{
    public interface IProfileViewService
    {
        Task ShowProfile(long chatId, IMessage? message, 
            UserDto user, CancellationToken ct);
    }
}
