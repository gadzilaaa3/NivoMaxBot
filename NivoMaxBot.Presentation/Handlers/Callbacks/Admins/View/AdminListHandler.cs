using MediatR;
using NivoMaxBot.Application.Features.Admins.Queries;
using NivoMaxBot.Messaging.Abstractions.Attachments.Inline;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.Routing;

namespace NivoMaxBot.Presentation.Handlers.Callbacks.Admins.View
{
    [CallbackRoute(AdminsRoutes.List)]
    public class AdminListHandler
    {
        private readonly IMediator _mediator;
        private readonly IMessengerClient _botClient;

        public AdminListHandler(IMediator mediator, IMessengerClient botClient)
        {
            _mediator = mediator;
            _botClient = botClient;
        }

        public async Task HandleAsync(ICallbackQuery query, CancellationToken ct)
        {
            var admins = await _mediator.Send(new GetAdminsQuery(), ct);
            var buttons = new List<InlineKeyboardButton[]>();

            foreach (var admin in admins.OrderBy(a => a.IsSuperAdmin))
            {
                var role = admin.IsSuperAdmin ? "🔹 Суперадмин" : "👤 Админ";
                var text = $"{admin.Username ?? admin.MaxId.ToString()} ({role})";
                buttons.Add(new[] { new InlineKeyboardButton(text, $"{AdminsRoutes.View}:{admin.Id}") });
            }

            buttons.Add(new[] { new InlineKeyboardButton("➕ Добавить администратора", AdminsRoutes.Add) });
            buttons.Add(new[] { new InlineKeyboardButton("🔙 Назад", MenuRoutes.AdminMode) });

            var keyboard = new InlineKeyboardMarkup(buttons);
            await _botClient.SendOrEditMessageAsync(query.Message.ChatId.Value, query.Message,
                "Список администраторов:", replyMarkup: keyboard, ct: ct);
        }
    }
}
