using MediatR;
using NivoMaxBot.Application.Features.Broadcast.Commands;
using NivoMaxBot.Application.Interfaces;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.Routing;
using NivoMaxBot.Presentation.Handlers.Messages.Broadcast;

namespace NivoMaxBot.Presentation.Handlers.Callbacks.Broadcast.Admin
{
    [CallbackRoute(AdminBroadcastRoutes.Confirm)]
    public class BroadcastConfirmHandler
    {
        private readonly IMediator _mediator;
        private readonly IUserStateService _userStateService;
        private readonly IMessengerClient _botClient;

        public BroadcastConfirmHandler(
            IMediator mediator,
            IUserStateService userStateService,
            IMessengerClient messengerBotClient)
        {
            _mediator = mediator;
            _userStateService = userStateService;
            _botClient = messengerBotClient;
        }

        public async Task HandleAsync(ICallbackQuery query, CancellationToken ct)
        {
            var userId = query.From.Id;
            var state = _userStateService.GetState(userId);
            if (state.CurrentAction != BroadcastMessageReceiverHandler.ActionName)
            {
                await _botClient.AnswerCallbackQueryAsync(query.Id, "Ошибка состояния", ct: ct);
                return;
            }

            var broadcastData = (BroadcastTypeHandler.BroadcastData)state.TypedData;
            var command = new SendBroadcastCommand
            {
                AdminChatId = query.Message.ChatId.Value,
                SourceChatId = broadcastData.SourceChatId,
                SourceMessageId = broadcastData.SourceMessageId,
                BroadcastType = broadcastData.BroadcastType
            };

            await _botClient.SendTextMessageAsync(query.Message.ChatId.Value, "⏳ Начинаю рассылку...", ct: ct);
            await _botClient.AnswerCallbackQueryAsync(query.Id, ct: ct);

            try
            {
                await _mediator.Send(command, ct);
            }
            catch (Exception ex)
            {
                await _botClient.SendTextMessageAsync(query.Message.ChatId.Value, $"❌ Ошибка: {ex.Message}", ct: ct);
            }
            finally
            {
                _userStateService.ClearState(userId);
            }
        }
    }
}
