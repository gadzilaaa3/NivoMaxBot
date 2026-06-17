using Max.Bot;
using Max.Bot.Configuration;
using Max.Bot.Polling;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NivoMaxBot.MaxMessaging.Adapters;
using NivoMaxBot.MaxMessaging.BackgroundServices;
using NivoMaxBot.MaxMessaging.Dispatchers;
using NivoMaxBot.MaxMessaging.Options;
using NivoMaxBot.MaxMessaging.Webhook;
using NivoMaxBot.Messaging.Abstractions.Types;

namespace NivoMaxBot.MaxMessaging
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddMaxMessaging(this IServiceCollection services, 
            IConfiguration configuration)
        {
            services.AddSingleton(services =>
            {
                var token = configuration["MaxBot:Token"];

                return token == null
                    ? throw new ArgumentNullException("MaxBotToken in appsettings is empty, please correct this")
                    : new MaxClient(new MaxBotOptions
                    {
                        Token = token,
                    });
            });

            // Привязываем настройки
            services.Configure<MaxOptions>(configuration.GetSection(MaxOptions.SectionName));

            // Регистрируем валидатор и конфигуратор
            services.AddScoped<MaxWebhookSecretValidator>();
            services.AddHostedService<WebhookConfigurator>();

            services.AddHttpClient();

            services.AddScoped<IMessengerClient>(provider =>
            {
                var maxClient = provider.GetRequiredService<MaxClient>();
                var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
                
                var configuration = provider.GetRequiredService<IConfiguration>();
                var token = configuration["MaxBot:Token"];
                var maxApiUrl = configuration.GetValue<string>("MaxApiUrl");

                ArgumentNullException.ThrowIfNullOrEmpty(token);
                ArgumentNullException.ThrowIfNullOrEmpty(maxApiUrl);

                return new MaxMessengerClient(maxClient, httpClientFactory, token, maxApiUrl);
            });
            services.AddScoped<IUpdateHandler, MaxUpdateHandler>();
            return services;
        }
    }
}
