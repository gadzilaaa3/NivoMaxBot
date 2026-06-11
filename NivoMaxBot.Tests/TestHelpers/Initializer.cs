using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NivoMaxBot.Application;
using NivoMaxBot.Infrastructure;

namespace NivoTelegramBot.Tests.TestHelpers
{
    internal class Initializer
    {
        internal IServiceProvider ServiceProvider { init; get; }
        internal Initializer()
        {
            IServiceCollection services = new ServiceCollection();

            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            var config = builder.Build();

            services.AddApplicationServices();
            services.AddInfrastructureServices(config);

            services.AddLogging();

            ServiceProvider = services.BuildServiceProvider();
        }
    }
}
