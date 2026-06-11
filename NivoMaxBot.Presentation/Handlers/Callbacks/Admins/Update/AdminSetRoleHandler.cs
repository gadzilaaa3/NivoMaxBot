using MediatR;
using NivoMaxBot.Application.Features.Admins.Commands.UpdateRole;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.ErrorHandling;
using NivoMaxBot.Messaging.Routing;
using NivoMaxBot.Presentation.Common.Keyboards.Menu;

namespace NivoMaxBot.Presentation.Handlers.Callbacks.Admins.Update
{
    [CallbackRoute($"{AdminsRoutes.SetRole}:{{adminId:int}}:{{role}}")]
    public class AdminSetRoleHandler
    {
        private readonly IMediator _mediator;
        private readonly IMessengerClient _botClient;
        private readonly IErrorHandler _errorHandler;
        private readonly IMenuBuilder _menuBuilder;

        public AdminSetRoleHandler(
            IMediator mediator,
            IMessengerClient telegramBotClient,
            IErrorHandler errorHandler,
            IMenuBuilder menuBuilder)
        {
            _mediator = mediator;
            _botClient = telegramBotClient;
            _errorHandler = errorHandler;
            _menuBuilder = menuBuilder;
        }

        public async Task HandleAsync(ICallbackQuery query, int adminId, string role, CancellationToken ct)
        {
            var isSuperAdmin = role == "super";
            var command = new UpdateAdminRoleCommand
            {
                AdminId = adminId,
                IsSuperAdmin = isSuperAdmin
            };

            var keyboard = _menuBuilder.AddControlButtons([], 
                $"{AdminsRoutes.View}:{adminId}", MenuType.Admin);

            try
            {
                var result = await _mediator.Send(command, ct);
                await _botClient.SendOrEditMessageAsync(query.Message.ChatId.Value, query.Message, 
                    $"✅ Роль обновлена.", replyMarkup: keyboard, ct: ct);
            }
            catch (Exception ex)
            {
                await _errorHandler.HandleError(query.Message.ChatId.Value, ex, keyboard, ct);
            }
        }
    }
}
