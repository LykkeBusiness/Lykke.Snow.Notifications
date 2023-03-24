using System;
using Lykke.Snow.Notifications.Domain.Model;
using Xunit;

namespace Lykke.Snow.Notifications.Tests
{
    public class DeviceRegistrationTests
    {
        [Fact]
        public void DeviceRegistration_Instantiation_HappyPath()
        {
            new DeviceRegistration(accountId: "any-account-id", deviceToken: "any-device-token", deviceId: "any-device-id", registeredOn: DateTime.UtcNow);
        }
        
        [Fact]
        public void DeviceRegistration_InvalidAccountId_ShouldThrowException()
        {
            Assert.Throws<ArgumentNullException>(() => new DeviceRegistration(accountId: "", deviceToken: "any-device-token", deviceId: "any-device-id", registeredOn: DateTime.UtcNow)); 
            Assert.Throws<ArgumentNullException>(() => new DeviceRegistration(accountId: null, deviceToken: "any-device-token", deviceId: "any-device-id", registeredOn: DateTime.UtcNow)); 
        }

        [Fact]
        public void DeviceRegistration_InvalidDeviceToken_ShouldThrowException()
        {
            Assert.Throws<ArgumentNullException>(() => new DeviceRegistration(accountId: "any-account-id", deviceToken: "", deviceId: "any-device-id", registeredOn: DateTime.UtcNow)); 
            Assert.Throws<ArgumentNullException>(() => new DeviceRegistration(accountId: "any-account-id", deviceToken: null, deviceId: "any-device-id", registeredOn: DateTime.UtcNow)); 
        }

        [Fact]
        public void DeviceRegistration_InvalidRegisteredOn_ShouldThrowException()
        {
            Assert.Throws<ArgumentException>(() => new DeviceRegistration(accountId: "any-account-id", deviceToken: "any-device-token", deviceId: "any-device-id", registeredOn: default(DateTime))); 
            Assert.Throws<ArgumentException>(() => new DeviceRegistration(accountId: "any-account-id", deviceToken: "any-device-token", deviceId: "any-device-id", registeredOn: DateTime.UtcNow.AddSeconds(1))); 
        }
    }
}
