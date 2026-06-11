using NivoMaxBot.Application.Interfaces;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.Routing;

namespace NivoMaxBot.Presentation.Handlers.Callbacks.Admins.Add
{
    [CallbackRoute(AdminsRoutes.Add)]
    public class AdminAddHandler
    {
        private readonly IUserStateService _userStateService;
        private readonly IMessengerClient _botClient;

        public const string ActionName = "AddingAdmin";

        public AdminAddHandler(
            IUserStateService userStateService,
            IMessengerClient telegramBotClient)
        {
            _userStateService = userStateService;
            _botClient = telegramBotClient;
        }

        public async Task HandleAsync(ICallbackQuery query, CancellationToken ct)
        {
            var userId = query.From.Id;
            var state = _userStateService.GetState(userId);
            state.CurrentAction = ActionName;

            var data = new AdminAddData() { CurrentStep = AddAdminStep.TelegramId };
            state.TypedData = data;

            _userStateService.SetState(userId, state);

            await _botClient.SendTextMessageAsync(query.Message.ChatId.Value,
                "Введите Telegram ID пользователя, которого хотите сделать администратором:", ct: ct);
        }

        public class AdminAddData
        {
            public AddAdminStep CurrentStep { get; set; }
            public long TelegramId { get; set; }
            public string? UserName { get; set; }
            public bool IsSuperAdmin { get; set; }
        }

        public enum AddAdminStep
        {
            TelegramId,
            UserName,
            Role,
        }
    }
}
