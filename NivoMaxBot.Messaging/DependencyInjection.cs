using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NivoMaxBot.Application.Interfaces;
using NivoMaxBot.Messaging.Abstractions.Types;
using NivoMaxBot.Messaging.Dispatchers;
using NivoMaxBot.Messaging.ErrorHandling;
using NivoMaxBot.Messaging.Handlers.Interfaces;
using System.Reflection;

namespace NivoMaxBot.Messaging
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddMessaging(this IServiceCollection services, Assembly handlersAssembly)
        {
            services.AddScoped<IErrorHandler, ApplicationErrorHandler>();
            services.AddScoped<IMessengerUpdateDispatcher>(sp =>
                new GeneralUpdateDispatcher(
                    sp.GetRequiredService<IEnumerable<IMessageHandler>>(),
                    sp.GetRequiredService<ILogger<GeneralUpdateDispatcher>>(),
                    sp,
                    sp.GetRequiredService<ICurrentUserService>(),
                    sp.GetRequiredService<IMessengerClient>(),
                    sp.GetRequiredService<IErrorHandler>(),
                    handlersAssembly
                ));
            return services;
        }
    }
}
