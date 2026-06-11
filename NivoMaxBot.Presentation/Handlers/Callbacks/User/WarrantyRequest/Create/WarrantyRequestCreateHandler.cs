using NivoMaxBot.Application.Interfaces;
using NivoMaxBot.Application.Services;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.Routing;

namespace NivoMaxBot.Presentation.Handlers.Callbacks.User.WarrantyRequest.Create
{
    [CallbackRoute(UserModeRoutes.WarrantyCreate)]
    public class WarrantyRequestCreateHandler
    {
        private readonly IUserService _userService;
        private readonly IUserStateService _userStateService;
        private readonly IMessengerClient _botClient;

        public const string ActionName = "CreatingWarrantyRequest";

        public WarrantyRequestCreateHandler(
            IUserService userService, 
            IUserStateService userStateService, 
            IMessengerClient botClient)
        {
            _userService = userService;
            _userStateService = userStateService;
            _botClient = botClient;
        }

        public async Task HandleAsync(ICallbackQuery query, CancellationToken ct)
        {
            var messengerId = query.From.Id;

            var state = _userStateService.GetState(messengerId);
            state.CurrentAction = ActionName;

            state.TypedData = new CreateWarrantyData
            {
                UserMessengerId = messengerId,
                CurrentStep = CreateWarrantyStep.ContactPerson,
            };
            _userStateService.SetState(messengerId, state);

            await _botClient.SendTextMessageAsync(query.Message.ChatId.Value,
                "Введите контактное лицо (ФИО):", ct: ct);
        }

        public class CreateWarrantyData
        {
            public long UserMessengerId { get; set; }
            public CreateWarrantyStep CurrentStep { get; set; }
            public string ContactPerson { get; set; } = string.Empty;
            public string ContactPhone { get; set; } = string.Empty;
            public string City { get; set; } = string.Empty;
            public string ProductSerialNumber { get; set; } = string.Empty;
            public string ProblemDescription { get; set; } = string.Empty;
            public string ContactEmail { get; set; } = string.Empty;
            public string? INN { get; set; }
        }

        public enum CreateWarrantyStep
        {
            ContactPerson,
            ContactPhone,
            City,
            ProductSerialNumber,
            INN,
            ContactEmail,
            ProblemDescription
        }
    }
}
