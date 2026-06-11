using MediatR;
using NivoMaxBot.Application.Common.Exceptions;
using NivoMaxBot.Application.Features.Categories.Commands.Update;
using NivoMaxBot.Application.Interfaces;
using NivoMaxBot.Messaging.Abstractions.Attachments.Inline;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.ErrorHandling;
using NivoMaxBot.Messaging.Handlers.Interfaces;
using NivoMaxBot.Presentation.Common.Keyboards.Menu;
using NivoMaxBot.Presentation.Handlers.Callbacks.Categories.Admin;
using static NivoMaxBot.Presentation.Handlers.Callbacks.Categories.Admin.Edit.CategoryEditHandler;

namespace NivoMaxBot.Presentation.Handlers.Messages.Categories.Admin
{
    public class CategoryEditMessageHandler : IMessageHandler
    {
        private readonly IUserStateService _userStateService;
        private readonly IMediator _mediator;
        private readonly IMessengerClient _botClient;
        private readonly IMenuBuilder _menuBuilder;
        private readonly IErrorHandler _errorHandler;

        public CategoryEditMessageHandler(
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
            var data = (EditCategoryData)state.TypedData;

            try
            {
                await (data.CurrentStep switch
                {
                    EditCategoryStep.Name => HandleNameStep(data, message, chatId, ct),
                    EditCategoryStep.ChooseParent => HandleParentChooseStep(data, message, chatId, userId, ct),
                    EditCategoryStep.Order => HandleOrderStep(data, message, chatId, userId, ct),
                    _ => throw new IncorrectStateException()
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

        private async Task HandleNameStep(EditCategoryData data, IMessage message, 
            long chatId, CancellationToken ct)
        {
            var newName = message.Text == "-" ? data.Original.Name : message.Text;
            data.Name = newName;
            data.CurrentStep = EditCategoryStep.ChooseParent;

            // Отправляем сообщение с кнопкой для выбора родителя
            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new[] { new InlineKeyboardButton("🌳 Выбрать родителя", 
                    $"{AdminCategoryRoutes.ParentSelection}:0:for:{data.Original.Id}") }
            });
            await _botClient.SendTextMessageAsync(chatId,
                $"Текущий родитель: {data.Original.ParentName ?? "корень"}\n" +
                $"Нажмите кнопку, чтобы выбрать родителя, или отправьте '-' чтобы оставить без изменений.",
                replyMarkup: keyboard,
                ct: ct);
        }

        private async Task HandleParentChooseStep(EditCategoryData data, IMessage message,
            long chatId, long userId, CancellationToken ct)
        {
            if (message.Text == "-")
            {
                // Сохраняем выбранный родитель
                data.ParentId = data.Original.ParentId;
                data.CurrentStep = EditCategoryStep.Order;

                await _botClient.SendTextMessageAsync(chatId,
                    $"Родитель выбран. Текущий порядок: {data.Original.Order}" +
                    $"\nВведите новый порядок (число) или отправьте '-' для пропуска:",
                    ct: ct);
            }
            else
            {
                // Если пользователь отправил текст, проигнорируем и напомним
                await _botClient.SendTextMessageAsync(chatId, 
                    "Пожалуйста, используйте кнопку для выбора родителя.", ct: ct);
            }
        }

        private async Task HandleOrderStep(EditCategoryData data, IMessage message,
            long chatId, long userId, CancellationToken ct)
        {
            int newOrder;
            if (message.Text == "-")
                newOrder = data.Original.Order;
            else if (!int.TryParse(message.Text, out newOrder))
            {
                await _botClient.SendTextMessageAsync(chatId, "Введите число или '-' для пропуска.", 
                    ct: ct);
                return;
            }
            data.Order = newOrder;

            // Отправляем команду на обновление
            var command = new UpdateCategoryCommand
            {
                Id = data.Original.Id,
                Name = data.Name,
                ParentId = data.ParentId ?? data.Original.ParentId,
                Order = newOrder
            };

            var result = await _mediator.Send(command, ct);
            await _botClient.SendTextMessageAsync(chatId, "✅ Категория обновлена.", ct: ct);

            // Возвращаемся к просмотру категории
            var viewCallback = $"{AdminCategoryRoutes.View}:{data.Original.Id}";
            var keyboard = _menuBuilder.AddControlButtons([], viewCallback, MenuType.Admin);
            await _botClient.SendTextMessageAsync(chatId, "Продолжить редактирование:",
                replyMarkup: keyboard, ct: ct);

            _userStateService.ClearState(userId);
        }
    }
}
