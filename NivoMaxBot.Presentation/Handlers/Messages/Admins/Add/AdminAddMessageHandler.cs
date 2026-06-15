using MediatR;
using NivoMaxBot.Application.Common.Exceptions;
using NivoMaxBot.Application.Interfaces;
using NivoMaxBot.Messaging.Abstractions.Attachments.Inline;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.ErrorHandling;
using NivoMaxBot.Messaging.Handlers.Interfaces;
using NivoMaxBot.Presentation.Common.Keyboards.Menu;
using NivoMaxBot.Presentation.Handlers.Callbacks.Admins;
using NivoMaxBot.Presentation.Handlers.Callbacks.Admins.Add;

namespace NivoMaxBot.Presentation.Handlers.Messages.Admins.Add
{
    public class AdminAddMessageHandler : IMessageHandler
    {
        private readonly IUserStateService _userStateService;
        private readonly IMediator _mediator;
        private readonly IMessengerClient _botClient;
        private readonly IMenuBuilder _menuBuilder;
        private readonly IErrorHandler _errorHandler;

        public AdminAddMessageHandler(
            IUserStateService userStateService,
            IMediator mediator,
            IMessengerClient messengerBotClient,
            IMenuBuilder menuBuilder,
            IErrorHandler errorHandler)
        {
            _userStateService = userStateService;
            _mediator = mediator;
            _botClient = messengerBotClient;
            _menuBuilder = menuBuilder;
            _errorHandler = errorHandler;
        }

        public bool CanHandle(IMessage message)
        {
            var state = _userStateService.GetState(message.From.Id);
            return state.CurrentAction == AdminAddHandler.ActionName;
        }

        public async Task HandleAsync(IMessage message, CancellationToken ct)
        {
            var userId = message.From.Id;
            var chatId = message.ChatId.Value;
            var state = _userStateService.GetState(userId);
            var data = (AdminAddHandler.AdminAddData)state.TypedData;

            try
            {
                await (data.CurrentStep switch
                {
                    AdminAddHandler.AddAdminStep.MessengerId => HandleMessengerIdStep(message, data, chatId, ct),
                    AdminAddHandler.AddAdminStep.UserName => HandleUserNameStep(message, data, chatId, ct),
                    _ => throw new IncorrectStateException(nameof(AdminAddMessageHandler)),
                });

                _userStateService.SetState(userId, state);
            }
            catch (Exception ex)
            {
                _userStateService.ClearState(userId);

                var keyboard = _menuBuilder.AddControlButtons([], AdminsRoutes.List,
                    MenuType.Admin);
                await _errorHandler.HandleError(chatId, ex, keyboard, ct);
            }
        }

        public async Task HandleUserNameStep(IMessage message,
            AdminAddHandler.AdminAddData data, long chatId, CancellationToken ct = default)
        {
            data.UserName = null;
            if (message.Text != "-")
            {
                data.UserName = message.Text;
            }

            data.CurrentStep = AdminAddHandler.AddAdminStep.Role;

            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new[] { new InlineKeyboardButton("🔹 Суперадмин", $"{AdminsRoutes.AddRole}:super") },
                new[] { new InlineKeyboardButton("👤 Обычный администратор", $"{AdminsRoutes.AddRole}:regular") }
            });

            await _botClient.SendTextMessageAsync(chatId, "Выберите роль:", replyMarkup: keyboard, ct: ct);
        }

        public async Task HandleMessengerIdStep(IMessage message, 
            AdminAddHandler.AdminAddData data, long chatId, CancellationToken ct = default)
        {
            if (!long.TryParse(message.Text, out var messengerId))
            {
                await _botClient.SendTextMessageAsync(chatId, "Неверный формат Messenger ID. Введите число.", ct: ct);
                return;
            }

            // Сохраняем ID и запрашиваем username
            data.MessengerId = messengerId;
            data.CurrentStep = AdminAddHandler.AddAdminStep.UserName;

            await _botClient.SendTextMessageAsync(chatId, "Введите username (или '-', чтобы пропустить):", ct: ct);
        }
    }
}