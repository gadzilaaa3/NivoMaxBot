using Max.Bot;
using Max.Bot.Configuration;
using Max.Bot.Polling;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NivoMaxBot.MaxMessaging.Adapters;
using NivoMaxBot.MaxMessaging.BackgroundServices;
using NivoMaxBot.MaxMessaging.Dispatchers;
using NivoMaxBot.Messaging.Abstractions.Types;

namespace NivoMaxBot.MaxMessaging
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddMaxMessaging(this IServiceCollection services)
        {
            services.AddScoped(services =>
            {
                var config = services.GetRequiredService<IConfiguration>();
                var token = config.GetValue<string>("MaxBotToken");

                return token == null
                    ? throw new ArgumentNullException("MaxBotToken in appsettings is empty, please correct this")
                    : new MaxClient(new MaxBotOptions
                    {
                        Token = token,
                    });
            });

            services.AddHttpClient();

            services.AddScoped<IMessengerClient>(provider =>
            {
                var maxClient = provider.GetRequiredService<MaxClient>();
                var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
                
                var configuration = provider.GetRequiredService<IConfiguration>();
                var token = configuration.GetValue<string>("MaxBotToken");
                var maxApiUrl = configuration.GetValue<string>("MaxApiUrl");

                ArgumentNullException.ThrowIfNullOrEmpty(token);
                ArgumentNullException.ThrowIfNullOrEmpty(maxApiUrl);

                return new MaxMessengerClient(maxClient, httpClientFactory, token, maxApiUrl);
            });
            services.AddScoped<IUpdateHandler, MaxUpdateHandler>();
            services.AddHostedService<MaxBotBackgroundService>();
            return services;
        }
    }
}
