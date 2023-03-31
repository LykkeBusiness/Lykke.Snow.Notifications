using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using FirebaseAdmin.Messaging;
using Lykke.Snow.FirebaseIntegration.Exceptions;
using Lykke.Snow.FirebaseIntegration.Interfaces;
using Lykke.Snow.Notifications.Domain.Enums;
using Lykke.Snow.Notifications.Domain.Model;
using Lykke.Snow.Notifications.DomainServices.Services;
using Moq;
using Xunit;

namespace Lykke.Snow.Notifications.Tests
{
    public class NotificationServiceTests
    {
        class NotificationMessageTestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] { new NotificationMessage("any-title", "any-body", NotificationType.DepositSucceeded, new Dictionary<string, string>()), "any-device-token" };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        class TargetingTestData : IEnumerable<object[]>
        {
            private readonly DeviceConfiguration _deviceConfiguration = new DeviceConfiguration("any-device-id",
                "any-account-id", "en",
                new List<DeviceConfiguration.Notification>
                {
                    new DeviceConfiguration.Notification("DepositSucceeded"),
                    new DeviceConfiguration.Notification("WithdrawalSucceeded"),
                    new DeviceConfiguration.Notification("AccountLocked"),
                    new DeviceConfiguration.Notification("AccountUnlocked", false),
                }.AsReadOnly());
            
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] { _deviceConfiguration, NotificationType.DepositSucceeded, true };
                yield return new object[] { _deviceConfiguration, NotificationType.WithdrawalSucceeded, true };
                yield return new object[] { _deviceConfiguration, NotificationType.AccountLocked, true };
                yield return new object[] { _deviceConfiguration, NotificationType.AccountUnlocked, false };
                yield return new object[] { _deviceConfiguration, NotificationType.Liquidation, false };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        class BuildNotificationMessageTestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] 
                { 
                    NotificationType.AccountLocked, "title", "body", 
                    new Dictionary<string, string>() 
                    {
                        { "key1", "value1" },
                        { "key2", "value2" },
                        { "key3", "value3" },
                    } 
                };

                yield return new object[] 
                { 
                    NotificationType.AccountUnlocked, "title2", "body2", 
                    new Dictionary<string, string>()
                };

                yield return new object[] 
                { 
                    NotificationType.Liquidation, "another-title", "another-body", 
                    new Dictionary<string, string>() 
                    {
                        { "key", "value" }
                    } 
                };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        #region NotificationMessage model test

        [Fact]
        public void InstantiateNotificationMessage_WithEmptyTitleAndBody_ShouldResultInException()
        {
            Assert.Throws<ArgumentNullException>(() => {
                new NotificationMessage(title: string.Empty, body: string.Empty, NotificationType.AccountLocked, new Dictionary<string, string>());
                new NotificationMessage(title: string.Empty, body: "some-body", NotificationType.Liquidation, new Dictionary<string, string>());
                new NotificationMessage(title: "some-title", body: string.Empty, NotificationType.MarginCall1, new Dictionary<string, string>());
            });
        }
        #endregion

        #region SendNotification
        [Fact]
        public void AttemptingSendingNotification_WithoutProvidingDeviceToken_ShouldThrowException()
        {
            var sut = CreateSut();
            
            Assert.ThrowsAsync<ArgumentNullException>(async () => {
                await sut.SendNotification(new NotificationMessage("any-title", "any-body", NotificationType.OrderExecuted, 
                    new Dictionary<string, string>()), string.Empty);
            });
        }

        [Theory]
        [ClassData(typeof(NotificationMessageTestData))]
        public async Task SendNotification_ShouldWrapTheInnerException(NotificationMessage message, string deviceToken)
        {
            var exceptionToBeThrown = new CannotSendNotificationException(
                new Message(), MessagingErrorCode.SenderIdMismatch, new Exception());

            var fcmIntegrationServiceMock = new Mock<IFcmIntegrationService>();
            fcmIntegrationServiceMock.Setup(x => x.SendNotification(It.IsAny<Message>())).Throws(exceptionToBeThrown);
            
            var sut = CreateSut(fcmIntegrationServiceMock.Object);

            var exception =
                await Assert.ThrowsAsync<CannotSendNotificationException>(() =>
                    sut.SendNotification(message, deviceToken));
            
            Assert.Equal(exceptionToBeThrown.Message, exception.Message);
            Assert.Equal(exceptionToBeThrown.Data, exception.Data);
        }
        #endregion

        #region MapToFcmMessage
        [Theory]
        [ClassData(typeof(NotificationMessageData))]
        public void MappingNotificationMessage_ToFcmMessage(NotificationMessage notificationMessage, string deviceToken)
        {
            var sut = CreateSut();
            
            var fcmMessage = sut.MapToFcmMessage(notificationMessage, deviceToken);
            
            Assert.Equal(expected: notificationMessage.Title, actual: fcmMessage.Notification.Title);
            Assert.Equal(expected: notificationMessage.Body, actual: fcmMessage.Notification.Body);
            Assert.Equal(expected: notificationMessage.KeyValueCollection, actual: fcmMessage.Data);
            Assert.Equal(expected: deviceToken, actual: fcmMessage.Token);
        }
        #endregion

        #region IsDeviceTargeted
        [Theory]
        [ClassData(typeof(TargetingTestData))]
        public void IsDeviceTargeted_ShouldReturnIfDeviceTargeted_ByDeviceConfiguration(
            DeviceConfiguration deviceConfiguration, 
            NotificationType notificationType, 
            bool expected)
        {
            var sut = CreateSut();
            
            var result = sut.IsDeviceTargeted(deviceConfiguration, notificationType);
            
            Assert.Equal(expected, result);
        }
        #endregion

        #region BuildNotificationMessage

        [Theory]
        [ClassData(typeof(BuildNotificationMessageTestData))]
        public void BuildNotificationMessage_ShouldBuildNotificationMessage_WithGivenParameters(NotificationType notificationType, string title, string body, Dictionary<string, string> keyValuePairs)
        {
            var sut = CreateSut();
            
            var result = sut.BuildNotificationMessage(notificationType, title, body, keyValuePairs);
            
            Assert.Equal(notificationType, result.NotificationType);
            Assert.Equal(title, result.Title);
            Assert.Equal(body, result.Body);
            Assert.Equal(keyValuePairs, result.KeyValueCollection);
        }
        
        [Fact]
        public void BuildNotificationMessage_ShouldThrowArgumentNullException_WhenTitleOrBodyIsNotProvided()
        {
            var sut = CreateSut();
            
            Assert.Throws<ArgumentNullException>(() => sut.BuildNotificationMessage(NotificationType.AccountLocked, null, null, new Dictionary<string, string>()));
            Assert.Throws<ArgumentNullException>(() => sut.BuildNotificationMessage(NotificationType.AccountLocked, "", null, new Dictionary<string, string>()));
            Assert.Throws<ArgumentNullException>(() => sut.BuildNotificationMessage(NotificationType.AccountLocked, null, "", new Dictionary<string, string>()));
            Assert.Throws<ArgumentNullException>(() => sut.BuildNotificationMessage(NotificationType.AccountLocked, "", "", new Dictionary<string, string>()));
        }


        #endregion
        
        private NotificationService CreateSut(IFcmIntegrationService fcmServiceArg = null)
        {
            var fcmService = new Mock<IFcmIntegrationService>().Object;

            if(fcmServiceArg != null)
            {
                fcmService = fcmServiceArg;
            }

            return new NotificationService(fcmService);
        }
    }
}
