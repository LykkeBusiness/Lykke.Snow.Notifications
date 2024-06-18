using System;
using AutoMapper;
using FsCheck;
using FsCheck.Xunit;
using Lykke.Snow.Notifications.Domain.Model;
using Lykke.Snow.Notifications.MappingProfiles;
using Lykke.Snow.Notifications.SqlRepositories.Entities;
using Xunit;

namespace Lykke.Snow.Notifications.Tests
{
    public class DeviceRegistrationTests
    {
        [Fact]
        public void DeviceRegistration_Instantiation_HappyPath()
        {
            _ = new DeviceRegistration(accountId: "any-account-id", 
                deviceToken: "any-device-token",
                deviceId: "any-device-id", 
                registeredOn: DateTime.UtcNow);
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
        
        [Property]
        public Property DeviceRegistration_Mapping_ToEntityAndBack_ShouldReturnSameObject()
        {
            var mapper = new MapperConfiguration(cfg => cfg.AddProfile(new MappingProfile()))
                .CreateMapper();
            
            return Prop.ForAll(Gens.DeviceRegistration.ToArbitrary(), origin =>
            {
                var entity = mapper.Map<DeviceRegistrationEntity>(origin);
                var mapped = mapper.Map<DeviceRegistration>(entity);
                return origin.Equals(mapped);
            });
        }
    }
}
