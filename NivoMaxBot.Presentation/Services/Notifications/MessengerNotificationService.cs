using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NivoMaxBot.Application.Features.Broadcast.Dtos;
using NivoMaxBot.Application.Features.ConsultationRequests.Dtos;
using NivoMaxBot.Application.Features.Orders.Dtos;
using NivoMaxBot.Application.Features.WarrantyRequest.Dtos;
using NivoMaxBot.Application.Interfaces;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.Abstractions.Types.Enums;
using NivoMaxBot.Shared.Helpers;

namespace NivoMaxBot.Presentation.Services.Notifications
{
    public class MessengerNotificationService : INotificationService
    {
        private readonly IMessengerClient _botClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<MessengerNotificationService> _logger;

        public MessengerNotificationService(
            IMessengerClient botClient,
            IConfiguration configuration,
            ILogger<MessengerNotificationService> logger)
        {
            _botClient = botClient;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendBroadcastToUser(BroadcastDto broadcast, CancellationToken cancellationToken = default)
        {
            await _botClient.CopyMessageAsync(
                    userId: broadcast.ToUserId,
                    fromChatId: broadcast.FromChatId,
                    messageId: broadcast.MessageId,
                    ct: cancellationToken);
        }

        public async Task SendMessageToUserAsync(long userId, string message, CancellationToken cancellationToken = default)
        {
            await _botClient.SendTextMessageToUserAsync(userId, message, ct: cancellationToken);
        }

        public async Task SendOrderNotification(OrderDto order, CancellationToken cancellationToken = default)
        {
            var chatId = _configuration.GetValue<long>("ManagerGroupChatId");
            if (chatId == 0)
            {
                _logger.LogWarning("ManagerGroupChatId not configured");
                return;
            }

            var text = $"📦 *Новый заказ #{order.Id}*\n\n" +
                       $"👤 Клиент: {order.CustomerName}\n" +
                       $"📞 Телефон: {order.CustomerPhone}\n" +
                       $"🏢 ИНН: {order.INN ?? "—"}\n" +
                       $"📧 Email: {order.CustomerEmail ?? "—"}\n" +
                       $"🕒 Дата: {MoscowTimeHelper.ToMoscowTimeString(order.CreatedAt)}\n\n" +
                       $"*Состав заказа:*\n";

            foreach (var item in order.Items)
            {
                text += $"{item.ProductName} x {item.Quantity} = {item.Total} руб.\n";
            }

            text += $"\n*Итого: {order.TotalAmount} руб.*";

            try
            {
                await _botClient.SendTextMessageAsync(chatId, text, textFormat: TextFormat.Markdown, 
                    ct: cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send order notification for order {OrderId}", order.Id);
            }
        }

        public async Task SendTextMessageAsync(long chatId, string message, CancellationToken cancellationToken = default)
        {
            await _botClient.SendTextMessageAsync(chatId, message, ct: cancellationToken);
        }

        public async Task SendWarrantyRequestNotification(WarrantyRequestDto request, CancellationToken cancellationToken = default)
        {
            var chatId = _configuration.GetValue<long>("ManagerGroupChatId");
            if (chatId == 0)
            {
                _logger.LogWarning("ManagerGroupChatId not configured");
                return;
            }

            var text = $"🔧 *Новая заявка по гарантии #{request.Id}*\n\n" +
                       $"🏢 ИНН: {request.INN ?? "—"}\n" +
                       $"🏙 Город: {request.City}\n" +
                       $"👤 Контактное лицо: {request.ContactPerson}\n" +
                       $"📞 Телефон: {request.ContactPhone}\n" +
                       $"📧 Email: {request.ContactEmail ?? "—"}\n" +
                       $"🔢 Серийный номер: {request.ProductSerialNumber ?? "—"}\n" +
                       $"🕒 Дата: {MoscowTimeHelper.ToMoscowTimeString(request.CreatedAt)}\n" +
                       $"📝 Описание проблемы: {request.ProblemDescription}";

            await _botClient.SendTextMessageAsync(chatId, text, 
                textFormat: TextFormat.Markdown, ct: cancellationToken);
        }

        public async Task SendConsultationRequestNotification(ConsultationRequestDto request, 
            CancellationToken cancellationToken = default)
        {
            var chatId = _configuration.GetValue<long>("ManagerGroupChatId");
            if (chatId == 0)
            {
                _logger.LogWarning("ManagerGroupChatId not configured. Сообщение не отправлено.");
                return;
            }

            var text = $"📞 *Новая заявка на консультацию #{request.Id}*\n\n" +
                       $"👤 Имя: {request.CustomerName}\n" +
                       $"🏙 Город: {request.City}\n" +
                       $"📞 Телефон: {request.PhoneNumber}\n" +
                       $"📝 Описание: {request.Description}\n" +
                       $"🕒 Дата: {MoscowTimeHelper.ToMoscowTimeString(request.CreatedAt)}";

            await _botClient.SendTextMessageAsync(chatId, text, 
                textFormat: TextFormat.Markdown, ct: cancellationToken);
        }
    }
}
