using NivoMaxBot.Messaging.Abstractions.Types;

namespace NivoMaxBot.Presentation.Services.User.Cart
{
    public interface ICartViewService
    {
        Task ShowCart(long chatId, IMessage message,
            int userId, int pageNumber, CancellationToken ct);
    }
}
