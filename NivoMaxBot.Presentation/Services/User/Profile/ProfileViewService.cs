using NivoMaxBot.Application.Features.Users.Dtos;
using NivoMaxBot.Messaging.Abstractions.Attachments.Inline;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.Abstractions.Types.Enums;

namespace NivoMaxBot.Presentation.Services.User.Profile
{
    public class ProfileViewService : IProfileViewService
    {
        private readonly IMessengerClient _botClient;

        public ProfileViewService(IMessengerClient botClient)
        {
            _botClient = botClient;
        }

        public async Task ShowProfile(long chatId, IMessage? message, UserDto user, CancellationToken ct)
        {
            var text = $"👤 *Ваш профиль*\n\n";

            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new[] { new InlineKeyboardButton("📦 Мои заказы", "profile:orders") },
                new[] { new InlineKeyboardButton("🔧 Мои заявки на ремонт", "profile:warranty") },
                new[] { new InlineKeyboardButton("🔙 Главное меню", "user_mode") } // возврат в главное меню
            });

            await _botClient.SendOrEditMessageAsync(chatId, message, 
                text, textFormat: TextFormat.Markdown, replyMarkup: keyboard, ct: ct);
        }
    }
}
