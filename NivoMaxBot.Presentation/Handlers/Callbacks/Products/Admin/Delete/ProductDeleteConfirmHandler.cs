using MediatR;
using NivoMaxBot.Application.Features.Products.Commands.Delete;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.Routing;
using NivoMaxBot.Presentation.Common.Keyboards.Menu;

namespace NivoMaxBot.Presentation.Handlers.Callbacks.Products.Admin.Delete
{
    [CallbackRoute("product:delete_confirm:{id:int}")]
    public class ProductDeleteConfirmHandler
    {
        private readonly IMediator _mediator;
        private readonly IMessengerClient _botClient;
        private readonly IMenuBuilder _menuBuilder;

        public ProductDeleteConfirmHandler(
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
            var result = await _mediator.Send(new DeleteProductCommand { Id = id }, ct);

            var keyboard = _menuBuilder.AddControlButtons([], "admin:products", MenuType.Admin);

            await _botClient.SendOrEditMessageAsync(query.Message.ChatId.Value, query.Message,
                "✅ Продукт удален.", replyMarkup: keyboard, ct: ct);
        }
    }
}
