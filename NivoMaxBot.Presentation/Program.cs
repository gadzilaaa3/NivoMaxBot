using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NivoMaxBot.Application;
using NivoMaxBot.Infrastructure;
using NivoMaxBot.Infrastructure.Data;
using NivoMaxBot.MaxMessaging;
using NivoMaxBot.Messaging;

namespace NivoMaxBot.Presentation
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            ApplyDbMigration(host);

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args);

            host.ConfigureLogging(logging =>
            {
                logging.AddFilter("LuckyPennySoftware.MediatR.License", LogLevel.None);
            });

            host.ConfigureAppConfiguration((context, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            });

            host.ConfigureServices((context, services) =>
            {
                services.AddApplicationServices();
                services.AddInfrastructureServices(context.Configuration);
                services.AddMessaging(typeof(Program).Assembly);
                services.AddMaxMessaging();
                services.AddPresentationServices();
            });

            return host;
        }

        public static void ApplyDbMigration(IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                try
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    dbContext.Database.Migrate();

                    logger.Log(LogLevel.Information, "\nDatabase migrations applied successfully\n");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Database migrations not applied");
                    Environment.Exit(1);
                }

            }
        }
    }
}