using NivoTelegramBot.Tests.TestHelpers;

namespace NivoMaxBot.Tests.TestHelpers
{
    public abstract class BaseTest
    {
        protected IServiceProvider ServiceProvider { get; }
        protected BaseTest()
        {
            ServiceProvider = new Initializer().ServiceProvider;
        }
    }
}
