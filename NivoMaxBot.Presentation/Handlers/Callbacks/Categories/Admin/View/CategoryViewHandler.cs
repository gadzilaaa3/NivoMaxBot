using MediatR;
using NivoMaxBot.Application.Common.Exceptions;
using NivoMaxBot.Application.Features.Categories.Queries.ById;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.Abstractions.Types.Enums;
using NivoMaxBot.Messaging.Routing;
using NivoMaxBot.Presentation.Common.Keyboards.Admin.Category;
using NivoMaxBot.Shared.Helpers;

namespace NivoMaxBot.Presentation.Handlers.Callbacks.Categories.Admin.View
{
    [CallbackRoute($"{AdminCategoryRoutes.View}:{{id:int}}")]
    public class CategoryViewHandler
    {
        private readonly IMediator _mediator;
        private readonly IMessengerClient _botClient;
        private readonly IAdminCategoryKeyboardFactory _keyboardFactory;

        public CategoryViewHandler(
            IMediator mediator, 
            IMessengerClient botClient, 
            IAdminCategoryKeyboardFactory keyboardFactory)
        {
            _mediator = mediator;
            _botClient = botClient;
            _keyboardFactory = keyboardFactory;
        }

        public async Task HandleAsync(ICallbackQuery query, int id, CancellationToken ct)
        {
            var chatId = query.Message.ChatId.Value;
            var category = await _mediator.Send(new GetCategoryByIdQuery { Id = id }, ct)
                ?? throw new NotFoundException("Категория не найдена.");

            var keyboard = _keyboardFactory.CreateCategoryViewKeyboard(id, category.ParentId);

            var text = $"ID: {category.Id}\n" +
                       $"📁 *{category.Name}*\n" +
                       $"Порядок: {category.Order}\n" +
                       $"🌳 Родитель: {category.ParentName ?? "корень"}\n" +
                       $"🕒 Дата создания: {MoscowTimeHelper.ToMoscowTimeString(category.CreatedAt)}\n" +
                       $"🕒 Дата обновления: {MoscowTimeHelper.ToMoscowTimeString(category.UpdatedAt)}";

            await _botClient.SendOrEditMessageAsync(chatId, query.Message, text, 
                textFormat: TextFormat.Markdown, replyMarkup: keyboard, ct: ct);
        }
    }
}
