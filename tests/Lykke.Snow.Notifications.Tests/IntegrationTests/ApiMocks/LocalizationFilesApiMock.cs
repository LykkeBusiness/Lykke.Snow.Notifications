using WireMock.Server;

namespace Lykke.Snow.Notifications.Tests.IntegrationTests.ApiMocks
{
    public class LocalizationFilesApiMock
    {
        public IApiEndpointMock GetActiveLocalizationFileEndpoint { get;  }
        public LocalizationFilesApiMock(WireMockServer server)
        {
            GetActiveLocalizationFileEndpoint = new GetActiveLocalizationFileEndpointMock(server);
        }
    }
}
