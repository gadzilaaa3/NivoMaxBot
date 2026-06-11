using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NivoMaxBot.Domain.Entities;
using NivoMaxBot.Domain.Interfaces.Repositories;
using NivoMaxBot.Infrastructure.Settings.Admin;

namespace NivoMaxBot.Infrastructure.Services.Initializers
{
    public class AdminInitializer : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly AdminSettings _adminSettings;
        private readonly ILogger<AdminInitializer> _logger;

        public AdminInitializer(
            IServiceProvider serviceProvider,
            IOptions<AdminSettings> adminSettings,
            ILogger<AdminInitializer> logger)
        {
            _serviceProvider = serviceProvider;
            _adminSettings = adminSettings.Value;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // Инициализация в фоне, чтобы не блокировать запуск
            _ = Task.Run(async () => await InitializeAdmins(cancellationToken), cancellationToken);
            await Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        private async Task InitializeAdmins(CancellationToken cancellationToken)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var adminRepository = scope.ServiceProvider.GetRequiredService<IAdminRepository>();

                var initialAdmins = _adminSettings.InitialAdmins;
                if (initialAdmins == null || initialAdmins.Count == 0)
                {
                    _logger.LogInformation("\nNo initial admins configured.\n");
                    return;
                }

                foreach (var initialAdmin in initialAdmins)
                {
                    var existingAdmin = await adminRepository.GetByMaxIdAsync(initialAdmin.MaxId, cancellationToken);
                    if (existingAdmin == null)
                    {
                        // Создание нового администратора
                        var newAdmin = new Admin
                        {
                            MaxId = initialAdmin.MaxId,
                            Username = initialAdmin.Username,
                            IsSuperAdmin = initialAdmin.IsSuperAdmin
                        };
                        await adminRepository.AddAsync(newAdmin, cancellationToken);
                        _logger.LogInformation("\nAdded admin with MaxId {MaxId}, IsSuperAdmin: {IsSuperAdmin}\n",
                            initialAdmin.MaxId, initialAdmin.IsSuperAdmin);
                    }
                    else
                    {
                        // Обновление существующего (изменились права или username)
                        if (existingAdmin.Username != initialAdmin.Username ||
                            existingAdmin.IsSuperAdmin != initialAdmin.IsSuperAdmin)
                        {
                            existingAdmin.Username = initialAdmin.Username;
                            existingAdmin.IsSuperAdmin = initialAdmin.IsSuperAdmin;
                            adminRepository.Update(existingAdmin);
                            _logger.LogInformation("\nUpdated admin with MaxId {MaxId}\n", initialAdmin.MaxId);
                        }
                    }
                }

                await adminRepository.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("\nInitial admins configured successfully.\n");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "\nFailed to initialize admins\n");
            }
        }
    }
}
