using MediatR;
using Microsoft.Extensions.Logging;
using NivoMaxBot.Application.Features.Products.Commands.BulkCreate;
using NivoMaxBot.Application.Features.Products.Dtos;
using NivoMaxBot.Application.Interfaces;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.Handlers.Interfaces;
using NivoMaxBot.Presentation.Common.Keyboards.Menu;
using NivoMaxBot.Presentation.Handlers.Callbacks.Products.Admin;
using System.Text.Json;
using static NivoMaxBot.Presentation.Handlers.Callbacks.Products.Admin.BulkAdd.ProductBulkAddHandler;

namespace NivoMaxBot.Presentation.Handlers.Messages.Products.Admin.BulkAdd
{
    public class ProductBulkAddMessageHandler : IMessageHandler
    {
        private readonly IUserStateService _userStateService;
        private readonly IMediator _mediator;
        private readonly IMessengerClient _botClient;
        private readonly ILogger<ProductBulkAddMessageHandler> _logger;
        private readonly IMenuBuilder _menuBuilder;

        public ProductBulkAddMessageHandler(
            IUserStateService userStateService, 
            IMediator mediator, 
            IMessengerClient botClient, 
            ILogger<ProductBulkAddMessageHandler> logger,
            IMenuBuilder menuBuilder)
        {
            _userStateService = userStateService;
            _mediator = mediator;
            _botClient = botClient;
            _logger = logger;
            _menuBuilder = menuBuilder;
        }

        public bool CanHandle(IMessage message)
        {
            var state = _userStateService.GetState(message.From.Id);
            return state.CurrentAction == ActionName;
        }

        public async Task HandleAsync(IMessage message, CancellationToken ct)
        {
            var userId = message.From.Id;
            var chatId = message.ChatId.Value;
            var state = _userStateService.GetState(userId);
            var data = (ProductBulkAddData)state.TypedData;

            var keyboard = _menuBuilder.AddControlButtons([], 
                $"{AdminProductRoutes.ProductsCategory}:{data.CategoryId}", MenuType.Admin);

            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    AllowTrailingCommas = true,
                };

                var products = JsonSerializer.Deserialize<IEnumerable<ProductImportDto>>(message.Text, options);
                if (products == null || !products.Any())
                {
                    await _botClient.SendTextMessageAsync(chatId, "❌ Неверный формат JSON или пустой массив.",
                        replyMarkup: keyboard, ct: ct);

                    _userStateService.ClearState(userId);
                    return;
                }

                var command = new BulkCreateProductsCommand
                {
                    CategoryId = data.CategoryId,
                    Products = products
                };

                var result = await _mediator.Send(command, ct);

                if (result.IsSuccess)
                {
                    await _botClient.SendTextMessageAsync(chatId, $"✅ Успешно добавлено {result.SuccessCount} товаров.",
                        replyMarkup: keyboard, ct: ct);
                }
                else
                {
                    var errorMessage = $"✅ Добавлено: {result.SuccessCount}\n❌ Ошибки:\n" + string.Join("\n", result.Errors);
                    await _botClient.SendTextMessageAsync(chatId, errorMessage,
                        replyMarkup: keyboard, ct: ct);
                }

                _userStateService.ClearState(userId);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Ошибка парсинга JSON");
                await _botClient.SendTextMessageAsync(chatId, "❌ Ошибка парсинга JSON. Проверьте формат.",
                    replyMarkup: keyboard, ct: ct);

                _userStateService.ClearState(userId);
            }
        }
    }
}
