using System;
using System.Collections.Generic;
using System.Linq;
using FsCheck;
using FsCheck.Xunit;
using Lykke.Snow.Notifications.Domain.Enums;
using Lykke.Snow.Notifications.Domain.Model;
using Xunit;

namespace Lykke.Snow.Notifications.Tests
{
    public class DeviceConfigurationTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Constructor_ThrowsArgumentNullException_WhenDeviceIdIsEmpty(string deviceId)
        {
            // Arrange, Act, Assert
            Assert.Throws<ArgumentNullException>(() => new DeviceConfiguration(deviceId, "accountId"));
        }
        
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Notification_Constructor_ThrowsArgumentNullException_WhenTypeIdIsEmpty(string type)
        {
            // Arrange, Act, Assert
            Assert.Throws<ArgumentNullException>(() => new DeviceConfiguration.Notification(type));
        }
        
        [Fact]
        public void Notification_Constructor_ThrowsArgumentNullException_WhenTypeIdIsNotSupported()
        {
            // Arrange, Act, Assert
            Assert.Throws<ArgumentException>(() => new DeviceConfiguration.Notification("not supported type"));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Constructor_ThrowsArgumentNullException_WhenAccountIdIsEmpty(string accountId)
        {
            // Arrange, Act, Assert
            Assert.Throws<ArgumentNullException>(() => new DeviceConfiguration("deviceId", accountId));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("not supported locale")]
        public void Constructor_ThrowsArgumentNullException_WhenLocaleIsEmptyOrNotSupported(string locale)
        {
            // Arrange, Act, Assert
            Assert.Throws<ArgumentException>(() => new DeviceConfiguration("deviceId", "accountId", locale));
        }
        
        [Theory]
        [InlineData(Locale.EN)]
        [InlineData(Locale.ES)]
        [InlineData(Locale.DE)]
        public void Constructor_Accepts_SupportedLocales(Locale locale)
        {
            // Arrange & Act
            var exception = Record.Exception(() => new DeviceConfiguration("deviceId", "accountId", locale.ToString()));

            // Assert
            Assert.Null(exception);
        }
        
        [Fact]
        public void Constructor_SetsDeviceIdProperty()
        {
            // Arrange, Act
            var deviceConfig = new DeviceConfiguration("deviceId", "accountId");

            // Assert
            Assert.Equal("deviceId", deviceConfig.DeviceId);
        }

        [Fact]
        public void Constructor_SetsAccountIdProperty()
        {
            // Arrange, Act
            var deviceConfig = new DeviceConfiguration("deviceId", "accountId");

            // Assert
            Assert.Equal("accountId", deviceConfig.AccountId);
        }

        [Fact]
        public void Constructor_SetsLocaleProperty()
        {
            // Arrange, Act
            var deviceConfig = new DeviceConfiguration("deviceId", "accountId");

            // Assert
            Assert.Equal(Locale.EN, deviceConfig.Locale);
        }

        [Fact]
        public void Constructor_SetsNotificationsPropertyToEmptyList_WhenNotificationsIsNull()
        {
            // Arrange, Act
            var deviceConfig = new DeviceConfiguration("deviceId", "accountId");

            // Assert
            Assert.Empty(deviceConfig.Notifications);
        }

        [Fact]
        public void Constructor_SetsNotificationsPropertyToGivenList_WhenNotificationsIsNotNull()
        {
            // Arrange
            var notifications = new List<DeviceConfiguration.Notification>
            {
                new DeviceConfiguration.Notification("DepositSucceeded"), 
                new DeviceConfiguration.Notification("WithdrawalSucceeded"),
            };

            // Act
            var deviceConfig = new DeviceConfiguration("deviceId", "accountId", "en", notifications);

            // Assert
            Assert.Equal(2, deviceConfig.Notifications.Count);
            Assert.Contains(deviceConfig.Notifications, n => n.Type == NotificationType.DepositSucceeded);
            Assert.Contains(deviceConfig.Notifications, n => n.Type == NotificationType.WithdrawalSucceeded);
        }
        
        [Fact]
        public void IsNotificationEnabled_ReturnsTrue_WhenNotificationIsEnabled()
        {
            // Arrange
            var deviceConfig = new DeviceConfiguration("deviceId", "accountId", "en",
                new List<DeviceConfiguration.Notification>
                {
                    new DeviceConfiguration.Notification("DepositSucceeded", false),
                    new DeviceConfiguration.Notification("WithdrawalSucceeded"),
                });

            // Act
            var result = deviceConfig.IsNotificationEnabled(NotificationType.WithdrawalSucceeded);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsNotificationEnabled_ReturnsFalse_WhenNotificationIsNotEnabled()
        {
            // Arrange
            var deviceConfig = new DeviceConfiguration("deviceId", "accountId", "en",
                new List<DeviceConfiguration.Notification>
                {
                    new DeviceConfiguration.Notification("DepositSucceeded"),
                    new DeviceConfiguration.Notification("WithdrawalSucceeded", false),
                });

            // Act
            var result = deviceConfig.IsNotificationEnabled(NotificationType.WithdrawalSucceeded);

            // Assert
            Assert.False(result);
        }
        
        [Fact]
        public void IsNotificationEnabled_ThrowsArgumentException_WhenNotificationTypeIsNotSupported()
        {
            // Arrange
            var deviceConfig = new DeviceConfiguration("deviceId", "accountId", "en",
                new List<DeviceConfiguration.Notification>
                {
                    new DeviceConfiguration.Notification("DepositSucceeded"),
                    new DeviceConfiguration.Notification("AccountLocked"),
                });

            // Act, Assert
            Assert.Throws<ArgumentException>(() =>
                deviceConfig.IsNotificationEnabled("not supported notification type"));
            
            // The code expects there is no notification type with value 1000
            Assert.Throws<ArgumentException>(() =>
                deviceConfig.IsNotificationEnabled((NotificationType)1000));
        }

        [Fact]
        public void IsNotificationEnabled_ReturnsTrue_WhenNotificationTypeAsStringIsEnabled()
        {
            // Arrange
            var deviceConfig = new DeviceConfiguration("deviceId", "accountId", "en",
                new List<DeviceConfiguration.Notification>
                {
                    new DeviceConfiguration.Notification("DepositSucceeded"),
                    new DeviceConfiguration.Notification("WithdrawalSucceeded"),
                });

            // Act
            var result = deviceConfig.IsNotificationEnabled("WithdrawalSucceeded");

            // Assert
            Assert.True(result);
        }

        
        [Fact]
        public void IsNotificationEnabled_ReturnsFalse_WhenNotificationTypeAsStringIsNotEnabled()
        {
            // Arrange
            var deviceConfig = new DeviceConfiguration("deviceId", "accountId", "en",
                new List<DeviceConfiguration.Notification>
                {
                    new DeviceConfiguration.Notification("DepositSucceeded"),
                    new DeviceConfiguration.Notification("WithdrawalSucceeded", false),
                });

            // Act
            var result = deviceConfig.IsNotificationEnabled("WithdrawalSucceeded");

            // Assert
            Assert.False(result);
        }
        
        [Property]
        public Property EnabledNotifications_ReturnsOnlyEnabledNotifications()
        {
            return Prop.ForAll(Gens.DeviceConfiguration.ToArbitrary(), dc =>
            {
                return dc.EnabledNotificationTypes.All(en => dc.Notifications.Any(n => n.Type == en && n.Enabled));
            });
        }
        
        [Fact]
        public void Default_CreatesDeviceConfigurationWithAllNotificationsEnabled()
        {
            // Arrange
            var deviceId = "test-device-id";
            var accountId = "test-account-id";
            var defaultLocale = Locale.EN;

            // Act
            var config = DeviceConfiguration.Default(deviceId, accountId);

            // Assert
            Assert.Equal(deviceId, config.DeviceId);
            Assert.Equal(accountId, config.AccountId);
            Assert.Equal(defaultLocale, config.Locale);
            Assert.NotNull(config.Notifications);

            // Ensure all notification types are enabled
            foreach (NotificationType type in Enum.GetValues(typeof(NotificationType)))
            {
                Assert.True(config.IsNotificationEnabled(type));
            }
        }

        [Property]
        public Property IsNotificationEnabled_WorksEqually_ForStringAndEnum()
        {
            return Prop.ForAll(Gens.DeviceConfiguration.ToArbitrary(), (DeviceConfiguration dc) =>
            {
                foreach (NotificationType type in Enum.GetValues(typeof(NotificationType)))
                {
                    var result1 = dc.IsNotificationEnabled(type);
                    var result2 = dc.IsNotificationEnabled(type.ToString());
                    if (result1 != result2)
                    {
                        return false;
                    }
                }

                return true;
            });
        }
        
        [Property]
        public Property NotitificationTypesAreUnique()
        {
            return Prop.ForAll(Gens.DeviceConfiguration.ToArbitrary(), (DeviceConfiguration dc) =>
            {
                var types = dc.Notifications.Select(n => n.Type).ToList();
                return types.Count == types.Distinct().Count();
            });
        }
    }
}
