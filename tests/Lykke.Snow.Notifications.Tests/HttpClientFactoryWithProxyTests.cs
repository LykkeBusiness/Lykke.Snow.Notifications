using System.Net.Http;
using Google.Apis.Http;
using Lykke.Snow.FirebaseIntegration;
using Xunit;

namespace Lykke.Snow.Notifications.Tests
{
    public class HttpClientFactoryWithProxyTests
    {
        [Fact]
        public void When_CredentialsPassed_InnerHandler_WithCredentials()
        {
            var proxyConfiguration = new ProxyConfiguration("address", "username", "password");

            var messageHandler = new HttpClientFactoryWithProxy(proxyConfiguration)
                .CreateHttpClient(new CreateHttpClientArgs())
                .MessageHandler;
            
            Assert.NotNull(messageHandler.InnerHandler);
            Assert.IsType<HttpClientHandler>(messageHandler.InnerHandler);
            
            var credentials = ((HttpClientHandler)messageHandler.InnerHandler!).Proxy!.Credentials;
            
            Assert.NotNull(credentials);
            Assert.Equal("username", credentials!.GetCredential(null, null)!.UserName);
            Assert.Equal("password", credentials!.GetCredential(null, null)!.Password);
        }
    }
}
