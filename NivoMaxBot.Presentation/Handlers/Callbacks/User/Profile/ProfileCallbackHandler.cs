using MediatR;
using NivoMaxBot.Application.Features.Users.Commands.Register;
using NivoMaxBot.Application.Features.Users.Queries.ByMaxId;
using NivoMaxBot.Application.Interfaces;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.Routing;
using NivoMaxBot.Presentation.Services.User.Profile;

namespace NivoMaxBot.Presentation.Handlers.Callbacks.User.Profile
{
    [CallbackRoute(UserModeRoutes.Profile)]
    public class ProfileCallbackHandler
    {
        private readonly IMediator _mediator;
        private readonly IMessengerClient _botClient;
        private readonly IUserStateService _userStateService;
        private readonly IProfileViewService _profileViewService;

        public ProfileCallbackHandler(
            IMediator mediator, 
            IMessengerClient botClient, 
            IUserStateService userStateService,
            IProfileViewService profileViewService)
        {
            _mediator = mediator;
            _botClient = botClient;
            _userStateService = userStateService;
            _profileViewService = profileViewService;
        }

        public async Task HandleAsync(ICallbackQuery query, CancellationToken ct)
        {
            var chatId = query.Message.ChatId.Value;
            var messengerId = query.From.Id;

            var user = await _mediator.Send(new GetUserByMaxIdQuery { MaxId = messengerId }, ct);

            if (user == null)
            {
                await StartRegistration(chatId, messengerId, ct);
                await _profileViewService.ShowProfile(chatId, query.Message, user, ct);
            }
            else
            {
                await _profileViewService.ShowProfile(chatId, query.Message, user, ct);
            }
        }

        private async Task StartRegistration(long chatId, long messengerId, CancellationToken ct)
        {
            var command = new RegisterUserCommand
            {
                MaxId = messengerId,
            };

            await _mediator.Send(command);
        }
    }
}
