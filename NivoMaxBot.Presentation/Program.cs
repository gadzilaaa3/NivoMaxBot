using Max.Bot;
using Max.Bot.Polling;
using Max.Bot.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NivoMaxBot.Application;
using NivoMaxBot.Infrastructure;
using NivoMaxBot.Infrastructure.Data;
using NivoMaxBot.MaxMessaging;
using NivoMaxBot.MaxMessaging.Dispatchers;
using NivoMaxBot.MaxMessaging.Webhook;
using NivoMaxBot.Messaging;

namespace NivoMaxBot.Presentation
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var webApp = CreateApplicationBuilder(args).Build();
            
            ApplyDbMigration(webApp);
            ConfigureWebApplication(webApp);

            webApp.Run();
        }

        static WebApplicationBuilder CreateApplicationBuilder(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Configuration
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            builder.Logging.AddFilter("LuckyPennySoftware.MediatR.License", LogLevel.None);

            builder.Host.ConfigureServices((context, services) =>
            {
                services.AddApplicationServices();
                services.AddInfrastructureServices(context.Configuration);
                services.AddMessaging(typeof(Program).Assembly);
                services.AddMaxMessaging(builder.Configuration);
                services.AddPresentationServices();
            });

            return builder;
        }

        static void ConfigureWebApplication(WebApplication webApp)
        {
            webApp.MapPost("/api/max/webhook", async ([FromBody] Update update, 
                HttpContext ctx, MaxClient maxClient,
                IUpdateHandler handler, IServiceProvider services) =>
            {
                // ПРОВЕРКА СЕКРЕТА
                var validator = ctx.RequestServices.GetRequiredService<MaxWebhookSecretValidator>();
                if (!await validator.ValidateRequestAsync(ctx.Request))
                {
                    return Results.Unauthorized();
                }

                if (update == null) return Results.BadRequest();

                await maxClient.ProcessWebhookAsync(update, handler, services);
                return Results.Ok();
            });
        }

        public static void ApplyDbMigration(WebApplication webApp)
        {
            using (var scope = webApp.Services.CreateScope())
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
