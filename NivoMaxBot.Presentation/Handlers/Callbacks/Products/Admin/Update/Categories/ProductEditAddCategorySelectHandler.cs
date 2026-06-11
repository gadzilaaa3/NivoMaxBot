using MediatR;
using NivoMaxBot.Application.Features.Products.Dtos;
using NivoMaxBot.Application.Interfaces;
using NivoMaxBot.Messaging.Abstractions.Attachments.Inline;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.Routing;

namespace NivoMaxBot.Presentation.Handlers.Callbacks.Products.Admin.Update.Categories
{
    [CallbackRoute("product:edit:addcategory:select:{categoryId:int}:{productId:int}")]
    public class ProductEditAddCategorySelectHandler
    {
        private readonly IUserStateService _userStateService;
        private readonly IMessengerClient _botClient;
        private readonly IMediator _mediator;

        public ProductEditAddCategorySelectHandler(
            IUserStateService userStateService,
            IMessengerClient telegramBotClient,
            IMediator mediator)
        {
            _userStateService = userStateService;
            _botClient = telegramBotClient;
            _mediator = mediator;
        }

        public async Task HandleAsync(ICallbackQuery query, int categoryId, int productId, CancellationToken ct)
        {
            var userId = query.From.Id;
            var state = _userStateService.GetState(userId);

            // Базовый список – либо из временных, либо из оригинала
            var original = state.Data["original"] as ProductDto;
            var categoryIds = state.Data.ContainsKey("tempCategoryIds")
                ? (IEnumerable<int>)state.Data["tempCategoryIds"]
                : original?.CategoryIds ?? [];

            if (!categoryIds.Contains(categoryId))
            {
                var cats = categoryIds.ToList();
                cats.Add(categoryId);

                state.Data["tempCategoryIds"] = cats;
                _userStateService.SetState(userId, state);
                await _botClient.SendTextMessageAsync(query.Message.ChatId.Value, 
                    "✅ Категория добавлена.", ct: ct);
            }
            else
            {
                await _botClient.SendTextMessageAsync(query.Message.ChatId.Value, 
                    "Эта категория уже добавлена.", ct: ct);
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
