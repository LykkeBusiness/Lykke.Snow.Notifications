using System;
using System.Collections.Generic;
using System.Linq;
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
            var deviceConfig = new DeviceConfiguration("deviceId", "accountId");

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
        
        [Fact]
        public void EnabledNotifications_ReturnsOnlyEnabledNotifications()
        {
            // Arrange
            var deviceConfig = new DeviceConfiguration("deviceId", "accountId", "en",
                new List<DeviceConfiguration.Notification>
                {
                    new DeviceConfiguration.Notification("DepositSucceeded", false),
                    new DeviceConfiguration.Notification("InboxMessage"),
                    new DeviceConfiguration.Notification("PositionClosed"),
                    new DeviceConfiguration.Notification("AccountLocked", false),
                });

            // Act
            var result = deviceConfig.EnabledNotifications.ToList();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, n => n.Type == NotificationType.InboxMessage && n.Enabled);
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
