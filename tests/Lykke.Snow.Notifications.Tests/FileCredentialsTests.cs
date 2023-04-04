using System;
using Lykke.Snow.FirebaseIntegration.Exceptions;
using Lykke.Snow.FirebaseIntegration.Services;
using Xunit;

namespace Lykke.Snow.Notifications.Tests
{
    public class FileCredentialsTests
    {
        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void InstantiatingWithNullCredentialsPath_ShouldThrow_ArgumentNullException(string credentialsPath)
        {
            Assert.Throws<ArgumentNullException>(() => CreateSut(credentialsPath));
        }

        [Fact]
        public void InstantiatingWithInvalidCredentialsPath_ShouldThrow_FirebaseCredentialsFileNotFoundException()
        {
            Assert.Throws<FirebaseCredentialsFileNotFoundException>(() => CreateSut("invalid-path"));
        }
        
        private static FileCredentials CreateSut(string credentialsPath)
        {
            return new FileCredentials(credentialsPath);
        }
    }
}
