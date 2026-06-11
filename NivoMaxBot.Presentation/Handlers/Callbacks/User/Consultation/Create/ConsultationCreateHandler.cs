using NivoMaxBot.Application.Interfaces;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.Routing;

namespace NivoMaxBot.Presentation.Handlers.Callbacks.User.Consultation.Create
{
    [CallbackRoute(UserModeRoutes.ConsultationCreate)]
    public class ConsultationCreateHandler
    {
        private readonly IMessengerClient _botClient;
        private readonly IUserStateService _userStateService;

        public const string ActionName = "CreatingConsultation";

        public ConsultationCreateHandler(
            IMessengerClient botClient,
            IUserStateService userStateService)
        {
            _botClient = botClient;
            _userStateService = userStateService;
        }

        public async Task HandleAsync(ICallbackQuery query, CancellationToken ct)
        {
            var messengerId = query.From.Id;

            var state = _userStateService.GetState(messengerId);
            state.CurrentAction = ActionName;

            state.TypedData = new CreateConsultationData
            {
                UserMessengerId = messengerId,
                CurrentStep = CreateConsultationStep.ContactName
            };
            _userStateService.SetState(messengerId, state);

            await _botClient.SendTextMessageAsync(query.Message.ChatId.Value,
                "Введите ваше имя отчество:", ct: ct);
        }

        public class CreateConsultationData
        {
            public long UserMessengerId { get; set; }
            public CreateConsultationStep CurrentStep { get; set; }
            public string ContactName { get; set; } = string.Empty;
            public string City { get; set; } = string.Empty;
            public string PhoneNumber { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
        }

        public enum CreateConsultationStep
        {
            ContactName,
            City,
            PhoneNumber,
            Description
        }
    }
}
