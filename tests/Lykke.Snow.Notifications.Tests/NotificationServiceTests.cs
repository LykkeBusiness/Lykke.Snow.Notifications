using Lykke.Snow.Notifications.Domain.Exceptions;
using Lykke.Snow.Notifications.Domain.Services;
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
        public void Initialized_ShouldBeFalse_BeforeInitializingTheService()
        {
            var sut = CreateSut();
            
            Assert.False(sut.IsInitialized);
        }

        [Fact]
        public void Initialized_ShouldBeTrue_AfterInitializingTheService()
        {
            var mockFcmService = new Mock<IFcmService>();
            mockFcmService.Setup(mock => mock.CreateApp(It.IsAny<string>())).Verifiable();

            var sut = CreateSut(mockFcmService.Object);
            
            sut.Initialize(credentialsFilePath: "any-path");
            
            Assert.True(sut.IsInitialized);
        }
        
        [Fact]
        public void CallingInitializeTwice_ShouldntBreakAnything()
        {
            var mockFcmService = new Mock<IFcmService>();
            mockFcmService.Setup(mock => mock.CreateApp(It.IsAny<string>())).Verifiable();
            
            var sut = CreateSut(mockFcmService.Object);
            
            sut.Initialize(credentialsFilePath: "any-path");
            sut.Initialize(credentialsFilePath: "any-path");
            
            Assert.True(sut.IsInitialized);
        }
        
        [Fact]
        public void Initialize_ShouldWrapInnerException()
        {
            var mockFcmService = new Mock<IFcmService>();
            mockFcmService.Setup(mock => mock.CreateApp(It.IsAny<string>()))
                .Throws(new FirebaseAppAlreadyExistsException(null));
        
            var sut = CreateSut(mockFcmService.Object);
            
            Assert.Throws<FirebaseAppAlreadyExistsException>(() => 
                sut.Initialize(credentialsFilePath: "any-path")
            );
        }
        
        [Fact]
        public void AttemptingSendingNotification_WithoutInitializing_ShouldResultInException()
        {
            var sut = CreateSut();
            
            Assert.Throws<NotificationServiceNotInitializedException>(() => {
                sut.SendNotification(new DummyNotificationType());
            });
        }

        private INotificationService CreateSut(IFcmService fcmServiceArg = null, ILogger<NotificationService> loggerArg = null)
        {
            IFcmService fcmService = new Mock<IFcmService>().Object;
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
