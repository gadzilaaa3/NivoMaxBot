using NivoMaxBot.Application.Interfaces;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.Routing;

namespace NivoMaxBot.Presentation.Handlers.Callbacks.Products.Admin.Create
{
    [CallbackRoute($"{AdminProductRoutes.Add}:{{categoryId:int}}")]
    public class ProductAddHandler
    {
        private readonly IUserStateService _userStateService;
        private readonly IMessengerClient _botClient;

        public const string ActionName = "AddingProduct";

        public ProductAddHandler(IUserStateService userStateService, IMessengerClient botClient)
        {
            _userStateService = userStateService;
            _botClient = botClient;
        }

        public async Task HandleAsync(ICallbackQuery query, int categoryId, CancellationToken ct)
        {
            var userId = query.From.Id;
            var state = _userStateService.GetState(userId);
            var data = new ProductAddData() { 
                CategoryId = categoryId, CurrentStep = ProductAddStep.Name };

            state.CurrentAction = ActionName;
            state.TypedData = data;

            _userStateService.SetState(userId, state);

            await _botClient.SendTextMessageAsync(query.Message.ChatId.Value, "Введите название товара:", ct: ct);
        }

        public class ProductAddData
        {
            public int CategoryId { get; set; }
            public string Name { get; set; } = string.Empty;
            public string? Description { get; set; }
            public decimal Price { get; set; }
            public int WarrantyInMonth { get; set; }
            public string? PhotoUrl { get; set; }
            public string? PhotoFileId { get; set; }
            public bool IsAvailable { get; set; }
            public ProductAddStep CurrentStep { get; set; }
        }

        public enum ProductAddStep
        {
            Name,
            Description,
            Price,
            Warranty,
            Photo,
            Available
        }
    }
}
