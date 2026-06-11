using MediatR;
using NivoMaxBot.Application.Common.Exceptions;
using NivoMaxBot.Application.Features.Products.Queries.ById;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.Abstractions.Types.Enums;
using NivoMaxBot.Messaging.Routing;
using NivoMaxBot.Presentation.Common.Keyboards.User.Catalog;

namespace NivoMaxBot.Presentation.Handlers.Callbacks.User.Catalog
{
    [CallbackRoute($"{UserCatalogRoutes.ProductView}:{{id:int}}")]
    public class UserProductViewHandler
    {
        private readonly IMediator _mediator;
        private readonly IMessengerClient _botClient;
        private readonly IUserProductViewKeyboardFactory _keyboardFactory;

        public UserProductViewHandler(
            IMediator mediator,
            IUserProductViewKeyboardFactory keyboardFactory,
            IMessengerClient botClient)
        {
            _mediator = mediator;
            _keyboardFactory = keyboardFactory;
            _botClient = botClient;
        }

        public async Task HandleAsync(ICallbackQuery query, int id, CancellationToken ct)
        {
            var chatId = query.Message.ChatId.Value;
            var product = await _mediator.Send(new GetProductByIdQuery { Id = id }, ct) 
                ?? throw new NotFoundException("Товар не найден.");

            // Определяем категорию для кнопки назад (первая категория)
            int? backCategoryId = product.CategoryIds.FirstOrDefault();
            var keyboard = _keyboardFactory.CreateProductViewKeyboard(id, backCategoryId);

            var warranty = product.WarrantyInMonths > 1000 ? $"Безусловная гарантия"
                : $"{product.WarrantyInMonths} мес.";

            var caption = $"🛒 *{product.Name}*\n" +
                          $"💰 Цена: {product.Price} руб.\n" +
                          $"📦 Гарантия: {warranty}\n" +
                          $"📝 Описание: {product.Description ?? "—"}\n" +
                          $"📁 Категории: {string.Join(", ", product.CategoryNames)}";

            if (!string.IsNullOrEmpty(product.PhotoUrl))
            {
                await _botClient.SendPhotoByUrlAsync(chatId,
                    url: product.PhotoUrl,
                    caption: caption,
                    textFormat: TextFormat.Markdown,
                    replyMarkup: keyboard,
                    ct: ct);
            }
            else if (!string.IsNullOrEmpty(product.PhotoMaxFileId))
            {
                await _botClient.SendPhotoByFileIdAsync(chatId, 
                    fileId: product.PhotoMaxFileId, 
                    caption: caption, 
                    textFormat: TextFormat.Markdown, 
                    replyMarkup: keyboard, 
                    ct: ct);
            }
            else
            {
                await _botClient.SendTextMessageAsync(chatId, 
                    caption, textFormat: TextFormat.Markdown, replyMarkup: keyboard, ct: ct);
            }
        }
    }
}
