using MediatR;
using NivoMaxBot.Application.Features.Admins.Queries;
using NivoMaxBot.Messaging.Abstractions.Attachments.Inline;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.Abstractions.Types.Enums;
using NivoMaxBot.Messaging.Routing;

namespace NivoMaxBot.Presentation.Handlers.Callbacks.Admins.View
{
    [CallbackRoute($"{AdminsRoutes.View}:{{adminId:int}}")]
    public class AdminViewHandler
    {
        private readonly IMediator _mediator;
        private readonly IMessengerClient _botClient;

        public AdminViewHandler(
            IMediator mediator,
            IMessengerClient telegramBotClient)
        {
            _botClient = telegramBotClient;
            _mediator = mediator;
        }

        public async Task HandleAsync(ICallbackQuery query, int adminId, CancellationToken ct)
        {
            var admins = await _mediator.Send(new GetAdminsQuery(), ct);
            var admin = admins.FirstOrDefault(a => a.Id == adminId);
            if (admin == null)
            {
                await _botClient.AnswerCallbackQueryAsync(query.Id, "Администратор не найден", ct: ct);
                return;
            }

            var text = $"<b>👤 Администратор</b>\n" +
                       $"ID: {admin.Id}\n" +
                       $"Telegram ID: {admin.MaxId}\n" +
                       $"Username: {System.Security.SecurityElement.Escape(admin.Username ?? "—")}\n" +
                       $"Роль: {(admin.IsSuperAdmin ? "Суперадмин" : "Обычный администратор")}";

            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new[] { new InlineKeyboardButton("✏️ Изменить роль", $"{AdminsRoutes.UpdateRole}:{admin.Id}") },
                new[] { new InlineKeyboardButton("❌ Удалить", $"{AdminsRoutes.Delete}:{admin.Id}") },
                new[] { new InlineKeyboardButton("🔙 Назад", AdminsRoutes.List) }
            });

            await _botClient.SendOrEditMessageAsync(query.Message.ChatId.Value, query.Message,
                text, textFormat: TextFormat.Html, replyMarkup: keyboard, ct: ct);
        }
    }
}
