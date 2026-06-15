using MediatR;
using NivoMaxBot.Application.Features.Products.Commands.Update;
using NivoMaxBot.Application.Features.Products.Dtos;
using NivoMaxBot.Application.Interfaces;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.Routing;
using NivoMaxBot.Presentation.Common.Keyboards.Menu;

namespace NivoMaxBot.Presentation.Handlers.Callbacks.Products.Admin.Update
{
    [CallbackRoute("product:edit:finish:{productId:int}")]
    public class ProductEditFinishHandler
    {
        private readonly IUserStateService _userStateService;
        private readonly IMediator _mediator;
        private readonly IMessengerClient _botClient;
        private readonly IMenuBuilder _menuBuilder;

        public ProductEditFinishHandler(
            IUserStateService userStateService,
            IMessengerClient messengerBotClient,
            IMediator mediator,
            IMenuBuilder menuBuilder)
        {
            _userStateService = userStateService;
            _botClient = messengerBotClient;
            _mediator = mediator;
            _menuBuilder = menuBuilder;
        }

        public async Task HandleAsync(ICallbackQuery query, int productId, CancellationToken ct)
        {
            var userId = query.From.Id;
            var state = _userStateService.GetState(userId);
            var original = state.Data["original"] as ProductDto;

            // Собираем финальный список категорий
            var categoryIds = state.Data.ContainsKey("tempCategoryIds")
                ? (IEnumerable<int>)state.Data["tempCategoryIds"]
                : original.CategoryIds;

            var command = new UpdateProductCommand
            {
                Id = productId,
                Name = (string)state.Data["name"],
                Description = (string?)state.Data["description"],
                Price = (decimal)state.Data["price"],
                WarrantyInMonths = (int)state.Data["warranty"],
                PhotoMaxFileId = (string?)state.Data["photo"],
                PhotoUrl = (string?)state.Data["photoUrl"],
                IsAvailable = (bool)state.Data["isAvailable"],
                CategoryIds = categoryIds
            };

            try
            {
                await _mediator.Send(command, ct);
                await _botClient.SendTextMessageAsync(query.Message.ChatId.Value, 
                    "✅ Товар обновлён.", ct: ct);

                // Возвращаемся к просмотру товара
                var viewCallback = $"product:view:{productId}";
                var keyboard = _menuBuilder.AddControlButtons([], viewCallback, MenuType.Admin);

                await _botClient.SendTextMessageAsync(query.Message.ChatId.Value, "Вернуться к товару: ",
                    replyMarkup: keyboard, ct: ct);
            }
            finally
            {
                _userStateService.ClearState(userId);
            }
        }
    }
}
