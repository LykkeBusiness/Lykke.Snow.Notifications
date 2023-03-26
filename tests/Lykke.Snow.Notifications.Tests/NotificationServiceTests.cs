using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using FirebaseAdmin.Messaging;
using Lykke.Snow.FirebaseIntegration.Exceptions;
using Lykke.Snow.FirebaseIntegration.Interfaces;
using Lykke.Snow.Notifications.Domain.Enums;
using Lykke.Snow.Notifications.Domain.Model;
using Lykke.Snow.Notifications.Domain.Services;
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

        [Fact]
        public void InstantiateNotificationMessage_WithEmptyTitleAndBody_ShouldResultInException()
        {
            Assert.Throws<ArgumentNullException>(() => {
                new NotificationMessage(title: string.Empty, body: string.Empty, NotificationType.AccountLocked, new Dictionary<string, string>());
                new NotificationMessage(title: string.Empty, body: "some-body", NotificationType.DepositFailed, new Dictionary<string, string>());
                new NotificationMessage(title: "some-title", body: string.Empty, NotificationType.MarginCall1, new Dictionary<string, string>());
            });
        }

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
        [ClassData(typeof(NotificationMessageData))]
        public void MappingNotificationMessage_ToFcmMessage(NotificationMessage notificationMessage, string deviceToken)
        {
            var sut = CreateSut();
            
            var fcmMessage = sut.MapToFcmMessage(notificationMessage, deviceToken);
            
            Assert.Equal(expected: notificationMessage.Title, actual: fcmMessage.Notification.Title);
            Assert.Equal(expected: notificationMessage.Body, actual: fcmMessage.Notification.Body);
            Assert.Equal(expected: notificationMessage.KeyValueCollection, actual: fcmMessage.Data);
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
        
        private NotificationService CreateSut(IFcmIntegrationService fcmServiceArg = null, ILocalizationService localizationServiceArg = null)
        {
            var fcmService = new Mock<IFcmIntegrationService>().Object;
            var localizationService = new Mock<ILocalizationService>().Object;

            if(fcmServiceArg != null)
            {
                fcmService = fcmServiceArg;
            }
            if(localizationServiceArg != null)
            {
                localizationService = localizationServiceArg; 
            }

            return new NotificationService(fcmService, localizationService);
        }
    }
}
