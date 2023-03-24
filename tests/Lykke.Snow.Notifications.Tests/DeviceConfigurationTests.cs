using System;
using System.Collections.Generic;
using System.Linq;
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
        public void Constructor_ThrowsArgumentNullException_WhenLocaleIsEmpty(string locale)
        {
            // Arrange, Act, Assert
            Assert.Throws<ArgumentNullException>(() => new DeviceConfiguration("deviceId", "accountId", locale));
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
            var deviceConfig = new DeviceConfiguration("deviceId", "accountId", "en");

            // Assert
            Assert.Equal("en", deviceConfig.Locale);
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
                new DeviceConfiguration.Notification("Deposit"), 
                new DeviceConfiguration.Notification("Withdrawal"),
            };

            // Act
            var deviceConfig = new DeviceConfiguration("deviceId", "accountId", "en", notifications);

            // Assert
            Assert.Equal(2, deviceConfig.Notifications.Count);
            Assert.Contains(deviceConfig.Notifications, n => n.Type == NotificationType.Deposit);
            Assert.Contains(deviceConfig.Notifications, n => n.Type == NotificationType.Withdrawal);
        }
        
        [Fact]
        public void IsNotificationEnabled_ReturnsTrue_WhenNotificationIsEnabled()
        {
            // Arrange
            var deviceConfig = new DeviceConfiguration("deviceId", "accountId", "en",
                new List<DeviceConfiguration.Notification>
                {
                    new DeviceConfiguration.Notification("Deposit", false),
                    new DeviceConfiguration.Notification("Withdrawal", true),
                });

            // Act
            var result = deviceConfig.IsNotificationEnabled(NotificationType.Withdrawal);

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
                    new DeviceConfiguration.Notification("Deposit"),
                    new DeviceConfiguration.Notification("Withdrawal", false),
                });

            // Act
            var result = deviceConfig.IsNotificationEnabled(NotificationType.Withdrawal);

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
                    new DeviceConfiguration.Notification("Deposit"),
                    new DeviceConfiguration.Notification("AccountLock", true),
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
                    new DeviceConfiguration.Notification("Deposit"),
                    new DeviceConfiguration.Notification("Withdrawal", true),
                });

            // Act
            var result = deviceConfig.IsNotificationEnabled("Withdrawal");

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
                    new DeviceConfiguration.Notification("Deposit"),
                    new DeviceConfiguration.Notification("Withdrawal", false),
                });

            // Act
            var result = deviceConfig.IsNotificationEnabled("Withdrawal");

            // Assert
            Assert.False(result);
        }
        
        [Fact]
        public void EnabledNotifications_ReturnsOnlyEnabledNotifications()
        {
            // Arrange
            var deviceConfig = new DeviceConfiguration("deviceId", "accountId", "en",
                new List<DeviceConfiguration.Notification>
                {
                    new DeviceConfiguration.Notification("Deposit", false),
                    new DeviceConfiguration.Notification("InboxMessages", true),
                    new DeviceConfiguration.Notification("PositionClosed", true),
                    new DeviceConfiguration.Notification("AccountLock", false),
                });

            // Act
            var result = deviceConfig.EnabledNotifications.ToList();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, n => n.Type == NotificationType.InboxMessages && n.Enabled);
            Assert.Contains(result, n => n.Type == NotificationType.PositionClosed && n.Enabled);
        }
        
        [Fact]
        public void Default_CreatesDeviceConfigurationWithAllNotificationsEnabled()
        {
            // Arrange
            var deviceId = "test-device-id";
            var accountId = "test-account-id";
            var defaultLocale = "en";

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
    }
}
