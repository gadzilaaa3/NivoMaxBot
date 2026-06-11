using MediatR;
using NivoMaxBot.Application.Common.Exceptions;
using NivoMaxBot.Application.Features.Categories.Dtos;
using NivoMaxBot.Application.Features.Categories.Queries.ById;
using NivoMaxBot.Application.Interfaces;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.Routing;

namespace NivoMaxBot.Presentation.Handlers.Callbacks.Categories.Admin.Edit
{
    [CallbackRoute($"{AdminCategoryRoutes.Edit}:{{id:int}}")]
    public class CategoryEditHandler
    {
        private readonly IMediator _mediator;
        private readonly IMessengerClient _botClient;
        private readonly IUserStateService _userStateService;

        public const string ActionName = "EditingCategory";

        public CategoryEditHandler(
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
            var category = await _mediator.Send(new GetCategoryByIdQuery { Id = id }, ct) 
                ?? throw new NotFoundException("Категория не найдена.");

            var userId = query.From.Id;
            var state = _userStateService.GetState(userId);
            var data = new EditCategoryData();

            state.CurrentAction = ActionName;
            state.EntityId = id;

            data.Original = category;
            data.CurrentStep = EditCategoryStep.Name;
            state.TypedData = data;

            _userStateService.SetState(userId, state);

            await _botClient.SendOrEditMessageAsync(query.Message.ChatId.Value, query.Message,
                $"Текущее название: {category.Name}\nВведите новое название (или отправьте '-' чтобы оставить без изменений):",
                ct: ct);
        }

        public class EditCategoryData
        {
            public CategoryDto Original { get; set; } = null!;
            public string Name { get; set; } = string.Empty;
            public int? ParentId { get; set; }
            public int Order { get; set; }
            public EditCategoryStep CurrentStep { get; set; }
        }

        public enum EditCategoryStep
        {
            Name,
            ChooseParent,
            Order,
        }
    }
}
