using NivoMaxBot.Application.Common.Exceptions;
using NivoMaxBot.Application.Interfaces;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.Routing;
using static NivoMaxBot.Presentation.Handlers.Callbacks.Categories.Admin.Edit.CategoryEditHandler;

namespace NivoMaxBot.Presentation.Handlers.Callbacks.Categories.Admin.Edit
{
    [CallbackRoute($"{AdminCategoryRoutes.SelectParent}:{{parentId:int}}:for:{{editId:int}}")]
    public class CategorySelectParentHandler
    {
        private readonly IUserStateService _userStateService;
        private readonly IMessengerClient _botClient;

        public CategorySelectParentHandler(IUserStateService userStateService, IMessengerClient botClient)
        {
            _userStateService = userStateService;
            _botClient = botClient;
        }

        public async Task HandleAsync(ICallbackQuery query, int parentId, int editId, CancellationToken ct)
        {
            var userId = query.From.Id;
            var state = _userStateService.GetState(userId);
            var data = (EditCategoryData)state.TypedData;

            if (state.CurrentAction == ActionName && data.Original.Id == editId)
            {
                data.ParentId = parentId == 0 ? null : parentId;
                data.CurrentStep = EditCategoryStep.Order; // переходим к шагу ввода порядка

                _userStateService.SetState(userId, state);
                await _botClient.SendOrEditMessageAsync(query.Message.ChatId.Value, query.Message,
                    $"Родитель выбран. Текущий порядок: {data.Original.Order}" +
                    $"\nВведите новый порядок (число) или отправьте '-' для пропуска:",
                    ct: ct);
            }
            else
            {
                throw new IncorrectStateException("Ошибка (редактирование категории): неверное " +
                    "состояние при выборе родителя категории.");
            }
        }
    }
}
