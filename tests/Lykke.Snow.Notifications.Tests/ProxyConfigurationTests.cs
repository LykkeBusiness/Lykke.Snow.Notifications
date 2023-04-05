using System;
using FsCheck;
using FsCheck.Xunit;
using Lykke.Snow.FirebaseIntegration;
using Xunit;

namespace Lykke.Snow.Notifications.Tests
{
    public class ProxyConfigurationTests
    {
        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Cannot_Create_With_Empty_Address(string address)
        {
            Assert.Throws<ArgumentNullException>(() => new ProxyConfiguration(address));
        }
        
        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Cannot_Create_With_Username_And_No_Password(string password)
        {
            Assert.Throws<ArgumentException>(() => new ProxyConfiguration("http://localhost", "username", password));
        }
        
        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Cannot_Create_With_Password_And_No_Username(string username)
        {
            Assert.Throws<ArgumentException>(() => new ProxyConfiguration("http://localhost", username, "password"));
        }

        [Property]
        public Property Can_Use_Credentials_If_Only_They_Are_Specified() =>
            Prop.ForAll(Gens.ProxyConfigurationWithCredentials.ToArbitrary(), cfg => cfg.CanUseCredentials);
    }
}
