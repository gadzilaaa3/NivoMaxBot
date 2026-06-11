using Microsoft.Extensions.DependencyInjection;
using NivoMaxBot.Domain.Interfaces.Repositories;
using NivoMaxBot.Tests.TestHelpers;

namespace NivoMaxBot.Tests
{
    public class UnitTest1 : BaseTest
    {
        [Fact]
        public async Task ShouldBeInitialAdmins()
        {
            var repo = ServiceProvider.GetRequiredService<IAdminRepository>();

            var admins = await repo.GetAllAsync();

            Assert.Single(admins);
        }
    }
}
