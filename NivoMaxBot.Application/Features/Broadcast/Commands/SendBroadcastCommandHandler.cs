using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NivoMaxBot.Application.Features.Broadcast.Dtos;
using NivoMaxBot.Application.Interfaces;
using NivoMaxBot.Domain.Entities;
using NivoMaxBot.Domain.Interfaces.Repositories;

namespace NivoMaxBot.Application.Features.Broadcast.Commands
{
    public class SendBroadcastCommandHandler : IRequestHandler<SendBroadcastCommand, BroadcastResult>
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public SendBroadcastCommandHandler(IServiceScopeFactory serviceScopeFactory)
        {
            _scopeFactory = serviceScopeFactory;
        }

        public async Task<BroadcastResult> Handle(SendBroadcastCommand request, CancellationToken cancellationToken)
        {
            var adminChatId = request.AdminChatId;
            var result = new BroadcastResult();

            // Запускаем рассылку в фоне
            _ = Task.Run(async () =>
            {
                using var scope = _scopeFactory.CreateScope();
                
                var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
                var orderRepository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();
                var warrantyRequestRepository = scope.ServiceProvider.GetRequiredService<IWarrantyRequestRepository>();
                var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

                var localResult = new BroadcastResult();
                try
                {
                    List<User> users;
                    if (request.BroadcastType == BroadcastType.All)
                    {
                        users = (await userRepository.GetAllAsync(CancellationToken.None)).ToList();
                    }
                    else
                    {
                        var userIdsWithOrders = await orderRepository.GetUserIdsWithOrdersAsync(CancellationToken.None);
                        var userIdsWithRepairs = await warrantyRequestRepository.GetUserIdsWithRequestsAsync(CancellationToken.None);
                        var activeUserIds = userIdsWithOrders.Union(userIdsWithRepairs).Distinct();
                        users = (await userRepository.GetByIdsAsync(activeUserIds, CancellationToken.None)).ToList();
                    }

                    var tasks = users.Select(async user =>
                    {
                        try
                        {
                            await notificationService.SendBroadcastToUser(
                                new BroadcastDto
                                {
                                    ToUserId = user.MaxId,
                                    FromChatId = request.SourceChatId,
                                    MessageId = request.SourceMessageId
                                },
                                CancellationToken.None);
                            Interlocked.Increment(ref localResult.SuccessCount);
                        }
                        catch (Exception ex)
                        {
                            Interlocked.Increment(ref localResult.FailedCount);
                            lock (localResult.Errors)
                            {
                                localResult.Errors.Add($"Пользователь {user.MaxId}: {ex.Message}");
                            }
                        }
                    });

                    await Task.WhenAll(tasks);
                }
                catch (Exception ex)
                {
                    localResult.Errors.Add($"Общая ошибка: {ex.Message}");
                }

                // Отправляем отчёт администратору после завершения
                try
                {
                    var report = $"✅ Рассылка завершена.\nУспешно: {localResult.SuccessCount}\nОшибок: {localResult.FailedCount}";
                    if (localResult.Errors.Any())
                        report += $"\nОшибки:\n{string.Join("\n", localResult.Errors)}";
                    await notificationService.SendTextMessageAsync(adminChatId, report, cancellationToken: CancellationToken.None);
                }
                catch (Exception ex)
                {
                    // Логирование ошибки отправки отчёта
                }
            }, cancellationToken);

            // Немедленный ответ администратору
            return result;
        }
    }
}
