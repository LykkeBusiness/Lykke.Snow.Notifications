using Lykke.Snow.FirebaseIntegration;
using Lykke.Snow.FirebaseIntegration.Interfaces;
using Lykke.Snow.FirebaseIntegration.Services;
using Moq;
using Xunit;

namespace Lykke.Snow.Notifications.Tests
{
    public class FcmOptionsFactoryTests
    {
        [Fact]
        public void When_ProxyConfiguration_Passed_Then_HttpClientFactoryWithProxy_IsUsed()
        {
            var options = new FcmOptionsFactory(Mock.Of<IGoogleCredentialsProvider>(),
                    new ProxyConfiguration("address", "username", "password"))
                .Create();
            
            Assert.NotNull(options.HttpClientFactory);
            Assert.IsType<HttpClientFactoryWithProxy>(options.HttpClientFactory);
        }
    }
}
