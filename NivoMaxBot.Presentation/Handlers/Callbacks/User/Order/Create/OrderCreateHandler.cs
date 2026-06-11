using NivoMaxBot.Application.Interfaces;
using NivoMaxBot.Application.Services;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.Routing;

namespace NivoMaxBot.Presentation.Handlers.Callbacks.User.Order.Create
{
    [CallbackRoute(UserModeRoutes.OrderCreate)]
    public class OrderCreateHandler
    {
        private readonly IMessengerClient _botClient;
        private readonly IUserService _userService;
        private readonly IUserStateService _userStateService;

        public const string ActionName = "CreatingOrder";

        public OrderCreateHandler(
            IMessengerClient botClient,
            IUserService userService, 
            IUserStateService userStateService)
        {
            _botClient = botClient;
            _userService = userService;
            _userStateService = userStateService;
        }

        public async Task HandleAsync(ICallbackQuery query, CancellationToken ct)
        {
            var messengerId = query.From.Id;

            var state = _userStateService.GetState(messengerId);
            state.CurrentAction = ActionName;

            state.TypedData = new CreateOrderData
            {
                UserMessengerId = messengerId,
                CurrentStep = CreateOrderStep.CustomerPerson,
            };
            _userStateService.SetState(messengerId, state);

            await _botClient.SendTextMessageAsync(query.Message.ChatId.Value,
                "Введите контактное лицо (ФИО):", ct: ct);
        }

        public class CreateOrderData
        {
            public long UserMessengerId { get; set; }
            public CreateOrderStep CurrentStep { get; set; }
            public string CustomerName { get; set; } = string.Empty;
            public string ContactPhone { get; set; } = string.Empty;
            public string? ContactEmail { get; set; }
            public string? INN { get; set; }
        }

        public enum CreateOrderStep
        {
            CustomerPerson,
            ContactPhone,
            ContactEmail,
            INN
        }
    }
}
