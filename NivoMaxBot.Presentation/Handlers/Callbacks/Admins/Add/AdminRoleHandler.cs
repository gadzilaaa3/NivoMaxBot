using MediatR;
using NivoMaxBot.Application.Features.Admins.Commands.Add;
using NivoMaxBot.Application.Interfaces;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.ErrorHandling;
using NivoMaxBot.Messaging.Routing;
using NivoMaxBot.Presentation.Common.Keyboards.Menu;

namespace NivoMaxBot.Presentation.Handlers.Callbacks.Admins.Add
{
    [CallbackRoute($"{AdminsRoutes.AddRole}:{{role}}")]
    public class AdminRoleHandler
    {
        private readonly IUserStateService _userStateService;
        private readonly IMediator _mediator;
        private readonly IMessengerClient _botClient;
        private readonly IMenuBuilder _menuBuilder;
        private readonly IErrorHandler _errorHandler;

        public AdminRoleHandler(
            IUserStateService userStateService,
            IMediator mediator,
            IMessengerClient telegramBotClient,
            IMenuBuilder menuBuilder,
            IErrorHandler errorHandler)
        {
            _userStateService = userStateService;
            _mediator = mediator;
            _botClient = telegramBotClient;
            _menuBuilder = menuBuilder;
            _errorHandler = errorHandler;
        }

        public async Task HandleAsync(ICallbackQuery query, string role, CancellationToken ct)
        {
            var userId = query.From.Id;
            var state = _userStateService.GetState(userId);
            var data = (AdminAddHandler.AdminAddData)state.TypedData;

            if (state.CurrentAction != AdminAddHandler.ActionName 
                || data.CurrentStep != AdminAddHandler.AddAdminStep.Role)
            {
                await _botClient.AnswerCallbackQueryAsync(query.Id, "Ошибка состояния", ct: ct);
                return;
            }

            var isSuperAdmin = role == "super";

            var command = new AddAdminCommand
            {
                MaxId = data.TelegramId,
                UserName = data.UserName,
                IsSuperAdmin = isSuperAdmin
            };

            var keyboard = _menuBuilder.AddControlButtons([], AdminsRoutes.List, MenuType.Admin);

            try
            {
                var result = await _mediator.Send(command, ct);
                await _botClient.SendTextMessageAsync(query.Message.ChatId.Value, $"✅ Администратор добавлен.",
                    replyMarkup: keyboard, ct: ct);
            }
            catch (Exception ex)
            {
                await _errorHandler.HandleError(query.Message.ChatId.Value, ex, keyboard, ct);
            }
            finally
            {
                _userStateService.ClearState(userId);
            }
        }
    }
}
