using MediatR;
using NivoMaxBot.Application.Common.Exceptions;
using NivoMaxBot.Application.Features.Categories.Commands.Create;
using NivoMaxBot.Application.Interfaces;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.ErrorHandling;
using NivoMaxBot.Messaging.Handlers.Interfaces;
using NivoMaxBot.Presentation.Common.Keyboards.Menu;
using NivoMaxBot.Presentation.Handlers.Callbacks.Categories.Admin;
using static NivoMaxBot.Presentation.Handlers.Callbacks.Categories.Admin.Create.CategoryAddHandler;

namespace NivoMaxBot.Presentation.Handlers.Messages.Categories.Admin
{
    public class CategoryAddMessageHandler : IMessageHandler
    {
        private readonly IUserStateService _userStateService;
        private readonly IMediator _mediator;
        private readonly IMessengerClient _botClient;
        private readonly IMenuBuilder _menuBuilder;
        private readonly IErrorHandler _errorHandler;

        public CategoryAddMessageHandler(
            IUserStateService userStateService, 
            IMediator mediator, 
            IMessengerClient botClient,
            IMenuBuilder menuBuilder,
            IErrorHandler errorHandler)
        {
            _userStateService = userStateService;
            _mediator = mediator;
            _botClient = botClient;
            _menuBuilder = menuBuilder;
            _errorHandler = errorHandler;
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
            var data = (AddCategoryData)state.TypedData;

            try
            {
                await (data.CurrentStep switch
                {
                    AddCategoryStep.Name => HandleNameStep(message, data, chatId, ct),
                    AddCategoryStep.Order => HandeOrderStep(message, data, chatId, userId, ct),
                    _ => throw new IncorrectStateException(nameof(CategoryAddMessageHandler)),
                });
                
                _userStateService.SetState(userId, state);
            }
            catch (Exception ex)
            {
                _userStateService.ClearState(userId);

                var keyboard = _menuBuilder.AddControlButtons([], AdminCategoryRoutes.List,
                    MenuType.Admin);
                await _errorHandler.HandleError(chatId, ex, keyboard, ct);
            }
        }

        private async Task HandleNameStep(IMessage message, 
            AddCategoryData data, long chatId, CancellationToken ct)
        {
            // Сохраняем название
            data.Name = message.Text;
            data.CurrentStep = AddCategoryStep.Order;
            await _botClient.SendTextMessageAsync(chatId, "Введите порядок сортировки (число, по умолчанию 0):", ct: ct);
        }

        private async Task HandeOrderStep(IMessage message,
            AddCategoryData data, long chatId, long userId, CancellationToken ct)
        {
            if (!int.TryParse(message.Text, out var order))
                order = 0;

            var command = new CreateCategoryCommand
            {
                Name = data.Name,
                ParentId = data.ParentId,
                Order = order
            };

            var categoryId = await _mediator.Send(command, ct);
            await _botClient.SendTextMessageAsync(chatId, $"✅ Категория создана с ID {categoryId}.", ct: ct);

            // Возвращаемся к списку категорий с тем же родителем
            string backCallBack = command.ParentId == null ? AdminCategoryRoutes.List 
                : $"{AdminCategoryRoutes.ParentChildrenList}:{command.ParentId}";
            var keyboard = _menuBuilder.AddControlButtons([], backCallBack, MenuType.Admin);

            await _botClient.SendTextMessageAsync(chatId, "Продолжить редактирование: ",
                replyMarkup: keyboard, ct: ct);

            _userStateService.ClearState(userId);
        }
    }
}