using WireMock.Server;

namespace Lykke.Snow.Notifications.Tests.IntegrationTests.ApiMocks
{
    public class AssetApiMock
    {
        public IApiEndpointMock GetLegacyAssetByIdEndpoint { get;  }
        public AssetApiMock(WireMockServer server)
        {
            GetLegacyAssetByIdEndpoint = new GetLegacyAssetByIdEndpointMock(server);
        }
    }
}
