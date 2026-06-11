using NivoMaxBot.Application.Interfaces;
using NivoMaxBot.Messaging.Abstractions.Attachments.Inline;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.Handlers.Interfaces;
using NivoMaxBot.Presentation.Handlers.Callbacks.Broadcast;
using NivoMaxBot.Presentation.Handlers.Callbacks.Broadcast.Admin;

namespace NivoMaxBot.Presentation.Handlers.Messages.Broadcast
{
    public class BroadcastMessageReceiverHandler : IMessageHandler
    {
        private readonly IUserStateService _userStateService;
        private readonly IMessengerClient _botClient;

        public const string ActionName = "BroadcastConfirmation";

        public BroadcastMessageReceiverHandler(
            IUserStateService userStateService, 
            IMessengerClient botClient)
        {
            _userStateService = userStateService;
            _botClient = botClient;
        }

        public bool CanHandle(IMessage message)
        {
            var state = _userStateService.GetState(message.From.Id);
            return state.CurrentAction == BroadcastTypeHandler.ActionName;
        }

        public async Task HandleAsync(IMessage message, CancellationToken ct)
        {
            var userId = message.From.Id;
            var chatId = message.ChatId.Value;
            var state = _userStateService.GetState(userId);
            var broadcastData = (BroadcastTypeHandler.BroadcastData)state.TypedData;

            // Сохраняем источник сообщения
            broadcastData.SourceChatId = chatId;
            broadcastData.SourceMessageId = message.MessageId;
            state.TypedData = broadcastData;

            // Запрашиваем подтверждение
            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new[] { new InlineKeyboardButton("✅ Да", AdminBroadcastRoutes.Confirm) },
                new[] { new InlineKeyboardButton("❌ Нет", MenuRoutes.AdminMode) }
            });

            await _botClient.SendTextMessageAsync(chatId, "Рассылаем это сообщение?", replyMarkup: keyboard, ct: ct);
            state.CurrentAction = ActionName;
            _userStateService.SetState(userId, state);
        }
    }
}
