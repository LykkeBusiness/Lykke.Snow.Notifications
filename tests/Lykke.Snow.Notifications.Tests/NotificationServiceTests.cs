using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using FirebaseAdmin.Messaging;
using Lykke.Snow.FirebaseIntegration.Exceptions;
using Lykke.Snow.FirebaseIntegration.Interfaces;
using Lykke.Snow.Notifications.Domain.Model;
using Lykke.Snow.Notifications.DomainServices.Services;
using Lykke.Snow.Notifications.Tests.Model;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Lykke.Snow.Notifications.Tests
{
    public class NotificationServiceTests
    {
        [Fact]
        public void InstantiateNotificationMessage_WithEmptyTitleAndBody_ShouldResultInException()
        {
            Assert.Throws<ArgumentNullException>(() => {
                new DummyMessage(title: null, body: null);
                new DummyMessage(title: string.Empty, body: string.Empty);
                new DummyMessage(title: string.Empty, body: "some-body");
                new DummyMessage(title: "some-title", body: string.Empty);
                new DummyMessage(title: null, body: "some-body");
                new DummyMessage(title: "some-title", body: null);
            });
        }

        [Fact]
        public void AttemptingSendingNotification_WithoutProvidingDeviceToken_ShouldThrowException()
        {
            var sut = CreateSut();
            
            Assert.ThrowsAsync<ArgumentNullException>(async () => {
                await sut.SendNotification(new DummyMessage("any-title", "any-body"), null);
                await sut.SendNotification(new DummyMessage("any-title", "any-body"), string.Empty);
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
        
        private NotificationService CreateSut(IFcmIntegrationService fcmServiceArg = null, ILogger<NotificationService> loggerArg = null)
        {
            IFcmIntegrationService fcmService = new Mock<IFcmIntegrationService>().Object;
            ILogger<NotificationService> logger = new Mock<ILogger<NotificationService>>().Object;

            if(fcmServiceArg != null)
            {
                fcmService = fcmServiceArg;
            }
            if(loggerArg != null)
            {
                logger = loggerArg;
            }

            return new NotificationService(fcmService, logger);
        }
    }
}
