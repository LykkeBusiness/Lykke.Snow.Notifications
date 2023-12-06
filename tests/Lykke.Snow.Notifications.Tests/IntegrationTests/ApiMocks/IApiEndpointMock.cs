namespace Lykke.Snow.Notifications.Tests.IntegrationTests.ApiMocks
{
    public interface IApiEndpointMock
    {
        void RespondWith(object body = null, int responseStatusCode = 200);
    }
}