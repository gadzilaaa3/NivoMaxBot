using NivoMaxBot.Application.Interfaces;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.Routing;

namespace NivoMaxBot.Presentation.Handlers.Callbacks.Categories.Admin.Create
{
    [CallbackRoute($"{AdminCategoryRoutes.Add}:{{parentId:int?}}")]
    public class CategoryAddHandler
    {
        private readonly IUserStateService _userStateService;
        private readonly IMessengerClient _botClient;

        public const string ActionName = "AddingCategory";

        public CategoryAddHandler(
            IUserStateService userStateService, 
            IMessengerClient botClient)
        {
            _userStateService = userStateService;
            _botClient = botClient;
        }

        public async Task HandleAsync(ICallbackQuery query, int? parentId, CancellationToken ct)
        {
            var userId = query.From.Id;
            
            var state = _userStateService.GetState(userId);
            var data = new AddCategoryData();

            data.ParentId = parentId;
            data.CurrentStep = AddCategoryStep.Name;

            state.CurrentAction = ActionName;

            state.TypedData = data;

            _userStateService.SetState(userId, state);

            await _botClient.SendTextMessageAsync(query.Message.ChatId.Value, 
                "Введите название категории: ", ct: ct);
        }

        public class AddCategoryData
        {
            public string Name { get; set; } = string.Empty;
            public int? ParentId { get; set; }
            public int Order { get; set; }
            public AddCategoryStep CurrentStep { get; set; }
        }

        public enum AddCategoryStep
        {
            Name,
            Order,
        }
    }
}
