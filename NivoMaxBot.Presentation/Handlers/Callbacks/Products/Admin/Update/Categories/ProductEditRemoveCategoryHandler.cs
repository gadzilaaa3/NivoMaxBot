using MediatR;
using NivoMaxBot.Application.Features.Categories.Queries.ById;
using NivoMaxBot.Application.Features.Products.Dtos;
using NivoMaxBot.Application.Interfaces;
using NivoMaxBot.Messaging.Abstractions.Attachments.Inline;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.Routing;

namespace NivoMaxBot.Presentation.Handlers.Callbacks.Products.Admin.Update.Categories
{
    [CallbackRoute("product:edit:removecategory:{productId:int}")]
    public class ProductEditRemoveCategoryHandler
    {
        private readonly IMediator _mediator;
        private readonly IMessengerClient _botClient;
        private readonly IUserStateService _userStateService;

        public ProductEditRemoveCategoryHandler(
            IMediator mediator, 
            IMessengerClient botClient, 
            IUserStateService userStateService)
        {
            _mediator = mediator;
            _botClient = botClient;
            _userStateService = userStateService;
        }

        public async Task HandleAsync(ICallbackQuery query, int productId, CancellationToken ct)
        {
            var userId = query.From.Id;
            var state = _userStateService.GetState(userId);

            // Получаем текущий список категорий из состояния или из оригинального продукта
            var categoryIds = state.Data.ContainsKey("tempCategoryIds")
                ? (IEnumerable<int>)state.Data["tempCategoryIds"]
                : (state.Data["original"] as ProductDto)?.CategoryIds ?? [];

            if (!categoryIds.Any())
            {
                await _botClient.SendTextMessageAsync(query.Message.ChatId.Value, 
                    "У продукта нет категорий для удаления.", ct: ct);
                return;
            }

            // Получаем информацию о категориях
            var categories = await _mediator.Send(new GetCategoriesByIdsQuery { Ids = categoryIds }, ct);
            var buttons = categories.Select(c => new[]
            {
                new InlineKeyboardButton($"❌ {c.Name}", $"product:edit:removecategory:select:{c.Id}:{productId}")
            }).ToList();

            // Кнопка "Назад"
            buttons.Add(new[] { new InlineKeyboardButton("🔙 Назад", $"product:edit:cancel:{productId}") });

            var keyboard = new InlineKeyboardMarkup(buttons);
            await _botClient.SendTextMessageAsync(query.Message.ChatId.Value, 
                "Выберите категорию для удаления:", replyMarkup: keyboard, ct: ct);
        }
    }
}
