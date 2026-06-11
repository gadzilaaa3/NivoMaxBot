using MediatR;
using NivoMaxBot.Application.Features.Categories.Commands.Delete;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.Routing;
using NivoMaxBot.Presentation.Common.Keyboards.Menu;

namespace NivoMaxBot.Presentation.Handlers.Callbacks.Categories.Admin.Delete
{
    [CallbackRoute($"{AdminCategoryRoutes.DeleteConfirm}:{{id:int}}")]
    public class CategoryDeleteConfirmHandler
    {
        private readonly IMediator _mediator;
        private readonly IMessengerClient _botClient;
        private readonly IMenuBuilder _menuBuilder;

        public CategoryDeleteConfirmHandler(
            IMediator mediator, 
            IMessengerClient botClient,
            IMenuBuilder menuBuilder)
        {
            _mediator = mediator;
            _botClient = botClient;
            _menuBuilder = menuBuilder;
        }

        public async Task HandleAsync(ICallbackQuery query, int id, CancellationToken ct)
        {
            var result = await _mediator.Send(new DeleteCategoryCommand { Id = id }, ct);
            var keyboard = _menuBuilder.AddControlButtons([], 
                AdminCategoryRoutes.List, MenuType.Admin);

            await _botClient.SendOrEditMessageAsync(query.Message.ChatId.Value, query.Message,
                "✅ Категория удалена.", replyMarkup: keyboard, ct: ct);
        }
    }
}
