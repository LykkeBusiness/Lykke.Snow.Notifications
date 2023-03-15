using System;
using Lykke.Snow.Notifications.Domain.Exceptions;
using Lykke.Snow.Notifications.Domain.Model;
using Lykke.Snow.Notifications.DomainServices.Services;
using Lykke.Snow.Notifications.Tests.Model;
using LykkeBiz.FirebaseIntegration.Exceptions;
using LykkeBiz.FirebaseIntegration.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Lykke.Snow.Notifications.Tests
{
    public class NotificationServiceTests
    {
        [Fact]
        public void Initialized_ShouldBeFalse_BeforeInitializingTheService()
        {
            var sut = CreateSut();
            
            Assert.False(sut.IsInitialized);
        }

        [Fact]
        public void Initialized_ShouldBeTrue_AfterInitializingTheService()
        {
            var mockFcmService = new Mock<IFcmIntegrationService>();
            mockFcmService.Setup(mock => mock.CreateApp()).Verifiable();

            var sut = CreateSut(mockFcmService.Object);
            
            sut.Initialize();
            
            Assert.True(sut.IsInitialized);
        }
        
        [Fact]
        public void CallingInitializeTwice_ShouldntBreakAnything()
        {
            var mockFcmService = new Mock<IFcmIntegrationService>();
            mockFcmService.Setup(mock => mock.CreateApp()).Verifiable();
            
            var sut = CreateSut(mockFcmService.Object);
            
            sut.Initialize();
            sut.Initialize();
            
            Assert.True(sut.IsInitialized);
            mockFcmService.Verify(mock => mock.CreateApp(), Times.Once);
        }
        
        [Fact]
        public void Initialize_ShouldWrapInnerException()
        {
            var mockFcmService = new Mock<IFcmIntegrationService>();
            mockFcmService.Setup(mock => mock.CreateApp())
                .Throws(new FirebaseAppAlreadyExistsException());
        
            var sut = CreateSut(mockFcmService.Object);
            
            Assert.Throws<FirebaseAppAlreadyExistsException>(() => 
                sut.Initialize()
            );
        }
        
        [Fact]
        public void AttemptingSendingNotification_WithoutInitializing_ShouldResultInException()
        {
            var sut = CreateSut();
            
            Assert.Throws<NotificationServiceNotInitializedException>(() => {
                sut.SendNotificationToSingleDevice(new DummyMessage("any-title", "any-body"), "any-device-token");
            });
        }
        
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
            
            sut.Initialize();
            
            Assert.Throws<ArgumentNullException>(() => {
                sut.SendNotificationToSingleDevice(new DummyMessage("any-title", "any-body"), null);
                sut.SendNotificationToSingleDevice(new DummyMessage("any-title", "any-body"), string.Empty);
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
            Assert.Equal(expected: notificationMessage.KeyValueBag, actual: fcmMessage.Data);
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
