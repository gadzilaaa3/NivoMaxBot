using NivoMaxBot.Application.Features.Broadcast.Dtos;
using NivoMaxBot.Application.Features.ConsultationRequests.Dtos;
using NivoMaxBot.Application.Features.Orders.Dtos;
using NivoMaxBot.Application.Features.WarrantyRequest.Dtos;

namespace NivoMaxBot.Application.Interfaces
{
    public interface INotificationService
    {
        Task SendOrderNotification(OrderDto order, CancellationToken cancellationToken = default);

        Task SendWarrantyRequestNotification(WarrantyRequestDto request, CancellationToken cancellationToken = default);

        Task SendBroadcastToUser(BroadcastDto broadcast, CancellationToken cancellationToken = default);

        Task SendTextMessageAsync(long chatId, string message, CancellationToken cancellationToken = default);

        Task SendMessageToUserAsync(long userId, string message, CancellationToken cancellationToken = default);

        Task SendConsultationRequestNotification(ConsultationRequestDto request, CancellationToken cancellationToken = default);
    }
}
