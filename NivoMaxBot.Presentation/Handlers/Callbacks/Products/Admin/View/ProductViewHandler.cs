using MediatR;
using NivoMaxBot.Application.Features.Products.Dtos;
using NivoMaxBot.Application.Features.Products.Queries.ById;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.Abstractions.Types.Enums;
using NivoMaxBot.Messaging.Routing;
using NivoMaxBot.Presentation.Common.Keyboards.Admin.Product;
using NivoMaxBot.Presentation.Common.Keyboards.Menu;

namespace NivoMaxBot.Presentation.Handlers.Callbacks.Products.Admin.View
{
    [CallbackRoute("product:view:{id:int}")]
    public class ProductViewHandler
    {
        private readonly IMediator _mediator;
        private readonly IMessengerClient _botClient;
        private readonly IProductViewKeyboardFactory _keyboardFactory;

        public ProductViewHandler(
            IMediator mediator, 
            IMessengerClient botClient, 
            IProductViewKeyboardFactory keyboardFactory)
        {
            _mediator = mediator;
            _botClient = botClient;
            _keyboardFactory = keyboardFactory;
        }

        public async Task HandleAsync(ICallbackQuery query, int id, CancellationToken ct)
        {
            var chatId = query.Message.ChatId.Value;
            var product = await _mediator.Send(new GetProductByIdQuery { Id = id }, ct);
            if (product == null)
            {
                await _botClient.SendTextMessageAsync(chatId, "Товар не найден.", ct: ct);
                return;
            }

            // Определяем категорию для кнопки "Назад" (первая категория из списка, если есть)
            int? backCategoryId = product.CategoryIds.FirstOrDefault();
            var keyboard = _keyboardFactory.CreateViewKeyboard(id, backCategoryId, MenuType.Admin);

            var caption = FormatProductCaption(product);

            if (!string.IsNullOrEmpty(product.PhotoUrl))
            {
                await _botClient.SendPhotoByUrlAsync(
                    chatId,
                    url: product.PhotoUrl,
                    caption: caption,
                    textFormat: TextFormat.Markdown,
                    replyMarkup: keyboard,
                    ct: ct);
            }
            else if (!string.IsNullOrEmpty(product.PhotoMaxFileId))
            {
                // Отправляем фото с подписью
                await _botClient.SendPhotoByFileIdAsync(
                    chatId,
                    fileId: product.PhotoMaxFileId,
                    caption: caption,
                    textFormat: TextFormat.Markdown,
                    replyMarkup: keyboard,
                    ct: ct);
            }
            else
            {
                // Отправляем только текст
                await _botClient.SendTextMessageAsync(
                    chatId,
                    caption,
                    textFormat: TextFormat.Markdown,
                    replyMarkup: keyboard,
                    ct: ct);
            }
        }

        private string FormatProductCaption(ProductDto product)
        {
            var categories = string.Join(", ", product.CategoryNames);
            var status = product.IsAvailable ? "✅ Доступен" : "❌ Недоступен";

            var warranty = product.WarrantyInMonths < 999 ? $"{product.WarrantyInMonths} мес."
                : "Пожизненная гарантия";

            return $"🛒 *{product.Name}*\n" +
                   $"💰 Цена: {product.Price} руб.\n" +
                   $"📦 Гарантия: {warranty}\n" +
                   $"📝 Описание: {product.Description ?? "—"}\n" +
                   $"📁 Категории: {categories}\n" +
                   $"📊 Статус: {status}\n";
        }
    }
}
