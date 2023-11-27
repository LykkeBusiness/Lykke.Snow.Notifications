using Lykke.Snow.Notifications.Tests.IntegrationTests.ApiMocks;
using WireMock.Server;
using WireMock.Settings;
using Xunit;

namespace Lykke.Snow.Notifications.Tests.IntegrationTests
{
    public class IntegrationTestsBase : IClassFixture<WebAppFactoryNoDependencies>
    {
        protected AssetApiMock AssetApi { get; }
        protected LocalizationFilesApiMock LocalizationFilesApi { get; }

        protected IntegrationTestsBase()
        {
            var settings = new WireMockServerSettings { Urls = new[] { "http://localhost:9095" } };
            var server = WireMockServer.Start(settings);

            AssetApi = new AssetApiMock(server);
            LocalizationFilesApi = new LocalizationFilesApiMock(server);
        }
    }
}
