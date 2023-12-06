using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace Lykke.Snow.Notifications.Tests.IntegrationTests.ApiMocks
{
    public class GetLegacyAssetByIdEndpointMock : IApiEndpointMock
    {
        private readonly WireMockServer _server;

        public GetLegacyAssetByIdEndpointMock(WireMockServer server)
        {
            _server = server;
        }
        public void RespondWith(object body = null, int responseStatusCode = 200)
        {
            _server.Given(Request.Create()
                    .WithPath(new WildcardMatcher("/api/assets/legacy/*"))
                    .UsingGet())
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(responseStatusCode)
                        .WithBodyAsJson(body));
        }
    }
}