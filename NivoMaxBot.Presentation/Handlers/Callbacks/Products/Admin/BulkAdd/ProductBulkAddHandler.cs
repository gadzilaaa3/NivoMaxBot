using NivoMaxBot.Application.Interfaces;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.Abstractions.Types.Enums;
using NivoMaxBot.Messaging.Routing;

namespace NivoMaxBot.Presentation.Handlers.Callbacks.Products.Admin.BulkAdd
{
    [CallbackRoute($"{AdminProductRoutes.BulkAdd}:{{categoryId:int}}")]
    public class ProductBulkAddHandler
    {
        private readonly IUserStateService _userStateService;
        private readonly IMessengerClient _botClient;

        public const string ActionName = "BulkAddProducts";

        public ProductBulkAddHandler(
            IUserStateService userStateService, 
            IMessengerClient botClient)
        {
            _userStateService = userStateService;
            _botClient = botClient;
        }

        public class ProductBulkAddData
        {
            public int CategoryId { get; set; }

            public ProductBulkAddStep CurrentStep { get; set; }
        }

        public enum ProductBulkAddStep
        {
            ParseBulk
        }

        public async Task HandleAsync(ICallbackQuery query, int categoryId, CancellationToken ct)
        {
            var userId = query.From.Id;
            var state = _userStateService.GetState(userId);
            var data = new ProductBulkAddData() { 
                CategoryId = categoryId, CurrentStep = ProductBulkAddStep.ParseBulk };

            state.CurrentAction = ActionName;
            state.TypedData = data;

            _userStateService.SetState(userId, state);
            
            await _botClient.SendOrEditMessageAsync(
                query.Message.ChatId.Value, query.Message,
                "Отправьте JSON с массивом товаров в формате:\n" +
                "```json\n" +
                "[\n" +
                "  {\n" +
                "    \"name\": \"Товар 1\",\t//(Название товара*: обязательно*)\n" +
                "    \"description\": \"Описание\",\t//(Не обязательное поле)\n" +
                "    \"price\": 1000,\t//(Стоимость товара в рублях*: обязательно*)\n" +
                "    \"photoUrl\": \"url\",\t//(Ссылка на фото: не обязательно)\n" +
                "    \"warrantyInMonths\": 12,\t//(Гарантия в месяцах*: Число больше 1000 будет считать пожизненной гарантией)\n" +
                "    \"isAvailable\": true\t//(Доступность в каталоге: не обязательно, по умолчанию true)\n" +
                "  }\n" +
                "]\n" +
                "```",
                textFormat: TextFormat.Markdown,
                ct: ct);
        }
    }
}
