using MediatR;
using NivoMaxBot.Application.Common.Exceptions;
using NivoMaxBot.Application.Features.Products.Queries.ById;
using NivoMaxBot.Application.Interfaces;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.Routing;

namespace NivoMaxBot.Presentation.Handlers.Callbacks.Products.Admin.Update
{
    [CallbackRoute("product:edit:{id:int}")]
    public class ProductEditHandler
    {
        private readonly IMediator _mediator;
        private readonly IMessengerClient _botClient;
        private readonly IUserStateService _userStateService;

        public ProductEditHandler(
            IMediator mediator, 
            IMessengerClient botClient, 
            IUserStateService userStateService)
        {
            _mediator = mediator;
            _botClient = botClient;
            _userStateService = userStateService;
        }

        public async Task HandleAsync(ICallbackQuery query, int id, CancellationToken ct)
        {
            var product = await _mediator.Send(new GetProductByIdQuery { Id = id }, ct) 
                ?? throw new NotFoundException($"Товар с ID {id} не найден.");

            var userId = query.From.Id;
            var state = _userStateService.GetState(userId);

            if (!string.IsNullOrEmpty(state.CurrentAction))
            {
                _userStateService.ClearState(userId);
            }

            state.CurrentAction = "EditingProduct";
            state.EntityId = id;
            state.Data["original"] = product; // сохраняем оригинал для отображения текущих значений
            state.Step = 1;
            _userStateService.SetState(userId, state);

            await _botClient.SendOrEditMessageAsync(
                query.Message.ChatId.Value, query.Message,
                $"Текущее название: {product.Name}\nВведите новое название (или отправьте '-' чтобы оставить без изменений):",
                ct: ct);
        }
    }
}
