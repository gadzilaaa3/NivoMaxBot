using NivoMaxBot.Application.Features.Products.Dtos;
using NivoMaxBot.Application.Interfaces;
using NivoMaxBot.Messaging.Abstractions.Attachments.Inline;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.Routing;

namespace NivoMaxBot.Presentation.Handlers.Callbacks.Products.Admin.Update.Categories
{
    [CallbackRoute("product:edit:removecategory:select:{categoryId:int}:{productId:int}")]
    public class ProductEditRemoveCategorySelectHandler
    {
        private readonly IUserStateService _userStateService;
        private readonly IMessengerClient _botClient;

        public ProductEditRemoveCategorySelectHandler(
            IUserStateService userStateService, 
            IMessengerClient botClient)
        {
            _userStateService = userStateService;
            _botClient = botClient;
        }

        public async Task HandleAsync(ICallbackQuery query, 
            int categoryId, int productId, CancellationToken ct)
        {
            var userId = query.From.Id;
            var state = _userStateService.GetState(userId);

            var categoryIds = state.Data.ContainsKey("tempCategoryIds")
                ? (IEnumerable<int>)state.Data["tempCategoryIds"]
                : (state.Data["original"] as ProductDto)?.CategoryIds ?? [];

            if (categoryIds.Contains(categoryId))
            {
                var cats = categoryIds.ToList();

                // Проверяем, не станет ли список пустым
                if (cats.Count < 2 && cats[0] == categoryId)
                {
                    await _botClient.SendTextMessageAsync(query.Message.ChatId.Value, 
                        "❌ Нельзя удалить последнюю категорию. У продукта должна быть хотя бы одна категория.", ct: ct);
                    return;
                }
                cats.Remove(categoryId);

                state.Data["tempCategoryIds"] = cats;
                _userStateService.SetState(userId, state);
                await _botClient.SendTextMessageAsync(query.Message.ChatId.Value, "✅ Категория удалена.", ct: ct);
            }
            else
            {
                await _botClient.SendTextMessageAsync(query.Message.ChatId.Value, "❌ Категория не найдена.", ct: ct);
            }

            // Возвращаемся к меню редактирования
            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new[] { new InlineKeyboardButton("➕ Добавить категорию", $"product:edit:addcategory:{productId}") },
                new[] { new InlineKeyboardButton("➖ Удалить категорию", $"product:edit:removecategory:{productId}") },
                new[] { new InlineKeyboardButton("✅ Завершить", $"product:edit:finish:{productId}") }
            });

            await _botClient.SendTextMessageAsync(query.Message.ChatId.Value,
                "Продолжить редактирование: ", replyMarkup: keyboard, ct: ct);
        }
    }
}
