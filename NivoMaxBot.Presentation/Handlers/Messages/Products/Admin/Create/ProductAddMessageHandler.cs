using MediatR;
using Microsoft.Extensions.Logging;
using NivoMaxBot.Application.Common.Exceptions;
using NivoMaxBot.Application.Features.Products.Commands.Create;
using NivoMaxBot.Application.Interfaces;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.Handlers.Interfaces;
using NivoMaxBot.Presentation.Common.Keyboards.Menu;
using static NivoMaxBot.Presentation.Handlers.Callbacks.Products.Admin.Create.ProductAddHandler;

namespace NivoMaxBot.Presentation.Handlers.Messages.Products.Admin.Create
{
    public class ProductAddMessageHandler : IMessageHandler
    {
        private readonly IUserStateService _userStateService;
        private readonly IMediator _mediator;
        private readonly IMessengerClient _botClient;
        private readonly ILogger<ProductAddMessageHandler> _logger;
        private readonly IMenuBuilder _menuBuilder;

        public ProductAddMessageHandler(
            IUserStateService userStateService, 
            IMediator mediator, 
            IMessengerClient botClient, 
            ILogger<ProductAddMessageHandler> logger,
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
            var data = (ProductAddData)state.TypedData;

            try
            {
                await (data.CurrentStep switch
                {
                    ProductAddStep.Name => HandleNameStep(data, message, chatId, ct),
                    ProductAddStep.Description => HandleDescriptionStep(data, message, chatId, ct),
                    ProductAddStep.Price => HandlePriceStep(data, message, chatId, ct),
                    ProductAddStep.Warranty => HandleWarrantyStep(data, message, chatId, ct),
                    ProductAddStep.Photo => HandlePhotoStep(data, message, chatId, ct),
                    ProductAddStep.Available => HandleAvailableStep(data, message, chatId, userId, ct),
                    _ => throw new IncorrectStateException()
                });
                
            }
            finally
            {
                _userStateService.SetState(userId, state);
            }
        }

        private async Task HandleNameStep(ProductAddData data, IMessage message, long chatId, 
            CancellationToken ct)
        {
            data.Name = message.Text;
            data.CurrentStep = ProductAddStep.Description;
            await _botClient.SendTextMessageAsync(chatId, 
                "Введите описание (или отправьте '-' чтобы пропустить):", ct: ct);
        }

        private async Task HandleDescriptionStep(ProductAddData data, IMessage message,
            long chatId, CancellationToken ct)
        {
            data.Description = message.Text == "-" ? null : message.Text;
            data.CurrentStep = ProductAddStep.Price;
            await _botClient.SendTextMessageAsync(chatId, "Введите цену (число):", ct: ct);
        }

        private async Task HandlePriceStep(ProductAddData data, IMessage message,
            long chatId, CancellationToken ct)
        {
            if (!decimal.TryParse(message.Text, out var price) || price <= 0)
            {
                await _botClient.SendTextMessageAsync(chatId,
                    "Цена должна быть положительным числом. Попробуйте ещё раз:", ct: ct);
                return;
            }
            data.Price = price;
            data.CurrentStep = ProductAddStep.Warranty;
            await _botClient.SendTextMessageAsync(chatId, "Введите гарантию в месяцах (число, 0 если нет, " +
                "числа больше 1000 будут отображаться как пожизненная гарантия):", ct: ct);
        }

        private async Task HandleWarrantyStep(ProductAddData data, IMessage message,
            long chatId, CancellationToken ct)
        {
            if (!int.TryParse(message.Text, out var warranty) || warranty < 0)
            {
                await _botClient.SendTextMessageAsync(chatId,
                    "Гарантия должна быть неотрицательным числом. Попробуйте ещё раз:", ct: ct);
                return;
            }
            data.WarrantyInMonth = warranty;
            data.CurrentStep = ProductAddStep.Photo;
            await _botClient.SendTextMessageAsync(chatId,
                "Отправьте фото или ссылку на фото товара (или отправьте '-' чтобы пропустить):", ct: ct);
        }

        private async Task HandlePhotoStep(ProductAddData data, IMessage message,
            long chatId, CancellationToken ct)
        {
            // Если сообщение содержит фото, берём file_id | url
            if (message.Photo?.Url != null)
            {
                data.PhotoUrl = message.Photo.Url;
            }
            else if (message.Photo?.FileId != null)
            {
                var fileId = message.Photo.FileId;
                data.PhotoFileId = fileId;
            }
            else if (message.Text == "-") { }
            else if (message.Text != null)
            {
                data.PhotoUrl = message.Text;
            }
            else
            {
                await _botClient.SendTextMessageAsync(chatId, "Пожалуйста, отправьте фото или ссылку ('-' для пропуска).", ct: ct);
                return;
            }
            data.CurrentStep = ProductAddStep.Available;
            await _botClient.SendTextMessageAsync(chatId,
                "Товар доступен для показа пользователям? (да/нет, по умолчанию да):", ct: ct);
        }

        private async Task HandleAvailableStep(ProductAddData data, IMessage message,
            long chatId, long userId, CancellationToken ct)
        {
            bool isAvailable = message.Text?.ToLower() != "нет";
            data.IsAvailable = isAvailable;

            // Собираем команду
            var command = new CreateProductCommand
            {
                Name = data.Name,
                Description = data.Description,
                Price = data.Price,
                WarrantyInMonths = data.WarrantyInMonth,
                PhotoMaxFileId = data.PhotoFileId,
                PhotoUrl = data.PhotoUrl,
                CategoryId = data.CategoryId,
                IsAvailable = data.IsAvailable,
            };

            var productId = await _mediator.Send(command, ct);
            await _botClient.SendTextMessageAsync(chatId, $"✅ Товар создан с ID {productId}.", ct: ct);
            _userStateService.ClearState(userId);

            // Возвращаемся к списку товаров категории
            var backCallback = $"products:category:{data.CategoryId}";
            var keyboard = _menuBuilder.AddControlButtons([], backCallback, MenuType.Admin);

            await _botClient.SendTextMessageAsync(chatId, "Вернуться к редактированию: ",
                replyMarkup: keyboard, ct: ct);
        }
    }
}
