using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Snow.Common.Model;
using Lykke.Snow.Notifications.Domain.Enums;
using Lykke.Snow.Notifications.Domain.Model;
using Lykke.Snow.Notifications.Domain.Repositories;
using Lykke.Snow.Notifications.Domain.Services;
using Lykke.Snow.Notifications.DomainServices.Services;
using Lykke.Snow.Notifications.Tests.Fakes;
using Meteor.Client.Models;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Lykke.Snow.Notifications.Tests
{
    class DeviceAndNotificationTestDataMultipleAccountId : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] 
            { 
                new List<DeviceRegistration> 
                {
                    new DeviceRegistration("account-id-1", "device-token-1", "device1", DateTime.UtcNow),
                    new DeviceRegistration("account-id-2", "device-token-2", "device2", DateTime.UtcNow),
                    new DeviceRegistration("account-id-3", "device-token-3", "device3", DateTime.UtcNow),
                    new DeviceRegistration("account-id-4", "device-token-4", "device4", DateTime.UtcNow),
                    new DeviceRegistration("account-id-5", "device-token-5", "device5", DateTime.UtcNow),
                },
                new List<string>
                {
                    "device3",
                    "device5",
                    "device1"
                },
                new NotificationMessage("title", "body", NotificationType.CashLocked, new Dictionary<string, string>())
            };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class MessagePreviewEventHandlerTests
    {
        #region Execution path tests

        [Fact]
        public async Task Handle_ShouldExitMethod_IfRecipientsIsNull()
        {
            var mockDeviceRegistrationService = new Mock<IDeviceRegistrationService>();
            var mockNotificationService = new Mock<INotificationService>();
            var mockDeviceConfigurationRepository = new Mock<IDeviceConfigurationRepository>();
            var mockLocalizationService = new Mock<ILocalizationService>();

            var sut = CreateSut(notificationServiceArg: mockNotificationService.Object,
                deviceRegistrationServiceArg: mockDeviceRegistrationService.Object,
                deviceConfigurationRepositoryArg: mockDeviceConfigurationRepository.Object,
                localizationServiceArg: mockLocalizationService.Object);
            
            var e = new MessagePreviewEvent();
            
            await sut.Handle(e);
            
            mockDeviceRegistrationService.Verify(x => x.GetDeviceRegistrationsAsync(It.IsAny<string[]>()), Times.Never);
            mockNotificationService.Verify(x => x.BuildNotificationMessage(It.IsAny<NotificationType>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()), Times.Never);
            mockDeviceConfigurationRepository.Verify(x => x.GetAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            mockNotificationService.Verify(x => x.IsDeviceTargeted(It.IsAny<DeviceConfiguration>(), It.IsAny<NotificationType>()), Times.Never);
            mockNotificationService.Verify(x => x.IsDeviceTargeted(It.IsAny<DeviceConfiguration>(), It.IsAny<NotificationType>()), Times.Never);
            mockNotificationService.Verify(x => x.SendNotification(It.IsAny<NotificationMessage>(), It.IsAny<string>()), Times.Never);
            mockLocalizationService.Verify(x => x.GetLocalizedTextAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IList<string>>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldExitMethod_IfEventIsNull()
        {
            var mockDeviceRegistrationService = new Mock<IDeviceRegistrationService>();
            var mockNotificationService = new Mock<INotificationService>();
            var mockDeviceConfigurationRepository = new Mock<IDeviceConfigurationRepository>();
            var mockLocalizationService = new Mock<ILocalizationService>();

            var sut = CreateSut(notificationServiceArg: mockNotificationService.Object,
                deviceRegistrationServiceArg: mockDeviceRegistrationService.Object,
                deviceConfigurationRepositoryArg: mockDeviceConfigurationRepository.Object,
                localizationServiceArg: mockLocalizationService.Object);
            
            await sut.Handle(null);
            
            mockDeviceRegistrationService.Verify(x => x.GetDeviceRegistrationsAsync(It.IsAny<string[]>()), Times.Never);
            mockNotificationService.Verify(x => x.BuildNotificationMessage(It.IsAny<NotificationType>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()), Times.Never);
            mockDeviceConfigurationRepository.Verify(x => x.GetAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            mockNotificationService.Verify(x => x.IsDeviceTargeted(It.IsAny<DeviceConfiguration>(), It.IsAny<NotificationType>()), Times.Never);
            mockNotificationService.Verify(x => x.IsDeviceTargeted(It.IsAny<DeviceConfiguration>(), It.IsAny<NotificationType>()), Times.Never);
            mockNotificationService.Verify(x => x.SendNotification(It.IsAny<NotificationMessage>(), It.IsAny<string>()), Times.Never);
            mockLocalizationService.Verify(x => x.GetLocalizedTextAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IList<string>>()), Times.Never);
        }
        
        [Fact]
        public async Task Handle_ShouldExitMethod_IfNotificationTypeNotFound()
        {
            var mockDeviceRegistrationService = new Mock<IDeviceRegistrationService>();
            var mockNotificationService = new Mock<INotificationService>();
            var mockDeviceConfigurationRepository = new Mock<IDeviceConfigurationRepository>();
            var mockLocalizationService = new Mock<ILocalizationService>();
            
            var sut = CreateSut(notificationServiceArg: mockNotificationService.Object,
                deviceRegistrationServiceArg: mockDeviceRegistrationService.Object,
                deviceConfigurationRepositoryArg: mockDeviceConfigurationRepository.Object,
                localizationServiceArg: mockLocalizationService.Object);            

            var e = new MessagePreviewEvent()
            {
                Recipients = new List<string> { "account-id-1" },
                // An event that doesn't map to a notification type.
                Event = MessageEventType.PriceAlertTriggered
            };
            
            // Simulate the mapping not containing the event type
            await sut.Handle(e);
            
            mockDeviceRegistrationService.Verify(x => x.GetDeviceRegistrationsAsync(It.IsAny<string[]>()), Times.Never);
            mockNotificationService.Verify(x => x.BuildNotificationMessage(It.IsAny<NotificationType>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()), Times.Never);
            mockDeviceConfigurationRepository.Verify(x => x.GetAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            mockNotificationService.Verify(x => x.IsDeviceTargeted(It.IsAny<DeviceConfiguration>(), It.IsAny<NotificationType>()), Times.Never);
            mockNotificationService.Verify(x => x.IsDeviceTargeted(It.IsAny<DeviceConfiguration>(), It.IsAny<NotificationType>()), Times.Never);
            mockNotificationService.Verify(x => x.SendNotification(It.IsAny<NotificationMessage>(), It.IsAny<string>()), Times.Never);
            mockLocalizationService.Verify(x => x.GetLocalizedTextAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IList<string>>()), Times.Never);
        }
        
        [Fact]
        public async Task Handle_ShouldExitMethod_IfGetDeviceRegistrationHasFailed()
        {
            var mockDeviceRegistrationService = new Mock<IDeviceRegistrationService>();
            var mockNotificationService = new Mock<INotificationService>();
            var mockDeviceConfigurationRepository = new Mock<IDeviceConfigurationRepository>();
            var mockLocalizationService = new Mock<ILocalizationService>();

            mockDeviceRegistrationService.Setup(x => x.GetDeviceRegistrationsAsync(It.IsAny<string[]>())).ReturnsAsync(new Result<IEnumerable<DeviceRegistration>, DeviceRegistrationErrorCode>(DeviceRegistrationErrorCode.DoesNotExist));

            var sut = CreateSut(notificationServiceArg: mockNotificationService.Object,
                deviceRegistrationServiceArg: mockDeviceRegistrationService.Object,
                deviceConfigurationRepositoryArg: mockDeviceConfigurationRepository.Object,
                localizationServiceArg: mockLocalizationService.Object);
            
            var e = new MessagePreviewEvent() 
            {
                Recipients = new List<string> { "account-id-1" }
            };
            
            await sut.Handle(e);

            mockDeviceRegistrationService.Verify(x => x.GetDeviceRegistrationsAsync(It.IsAny<string[]>()), Times.Once);
            mockNotificationService.Verify(x => x.BuildNotificationMessage(It.IsAny<NotificationType>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()), Times.Never);
            mockDeviceConfigurationRepository.Verify(x => x.GetAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            mockNotificationService.Verify(x => x.IsDeviceTargeted(It.IsAny<DeviceConfiguration>(), It.IsAny<NotificationType>()), Times.Never);
            mockNotificationService.Verify(x => x.IsDeviceTargeted(It.IsAny<DeviceConfiguration>(), It.IsAny<NotificationType>()), Times.Never);
            mockNotificationService.Verify(x => x.SendNotification(It.IsAny<NotificationMessage>(), It.IsAny<string>()), Times.Never);
            mockLocalizationService.Verify(x => x.GetLocalizedTextAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IList<string>>()), Times.Never);
        }

        [Theory]
        [ClassData(typeof(DeviceRegistrationsWithMissingConfigurationTestData))]
        public async Task Handle_ShouldSkipSending_IfDeviceConfigurationWasNotFound(List<DeviceRegistration> deviceRegistrations,
            List<string> deviceIdsMissingConfiguration)
        {
            var mockDeviceRegistrationService = new Mock<IDeviceRegistrationService>();
            mockDeviceRegistrationService.Setup(x => x.GetDeviceRegistrationsAsync(It.IsAny<string[]>())).ReturnsAsync(deviceRegistrations);

            var mockNotificationService = new Mock<INotificationService>();
            mockNotificationService.Setup(x => x.IsDeviceTargeted(It.IsAny<DeviceConfiguration>(), It.IsAny<NotificationType>())).Returns(true);

            var mockDeviceConfigurationRepository = new Mock<IDeviceConfigurationRepository>();
            mockDeviceConfigurationRepository.Setup(x => x.GetAsync(It.Is<string>(d => deviceIdsMissingConfiguration.Contains(d)), It.IsAny<string>()))
                .Returns(Task.FromResult<DeviceConfiguration>(null));

            mockDeviceConfigurationRepository.Setup(x => x.GetAsync(It.Is<string>(d => !deviceIdsMissingConfiguration.Contains(d)), It.IsAny<string>()))
                .Returns(Task.FromResult<DeviceConfiguration>(new DeviceConfiguration("device-id", "account-id")));

            var mockLocalizationService = new Mock<ILocalizationService>();

            var sut = CreateSut(notificationServiceArg: mockNotificationService.Object,
                deviceRegistrationServiceArg: mockDeviceRegistrationService.Object,
                deviceConfigurationRepositoryArg: mockDeviceConfigurationRepository.Object,
                localizationServiceArg: mockLocalizationService.Object);

            var e = new MessagePreviewEvent() 
            {
                Recipients = new List<string> { "account-id-1" },
                Subject = "subject",
                Content = "content"
            };
            
            await sut.Handle(e);

            var activeDeviceCount = deviceRegistrations.Count - deviceIdsMissingConfiguration.Count;

            mockDeviceConfigurationRepository.Verify(x => x.GetAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(deviceRegistrations.Count));
            mockNotificationService.Verify(x => x.BuildNotificationMessage(It.IsAny<NotificationType>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()), Times.Exactly(activeDeviceCount));
            mockNotificationService.Verify(x => x.IsDeviceTargeted(It.IsAny<DeviceConfiguration>(), It.IsAny<NotificationType>()), Times.Exactly(activeDeviceCount));
            mockNotificationService.Verify(x => x.SendNotification(It.IsAny<NotificationMessage>(), It.IsAny<string>()), Times.Exactly(activeDeviceCount));
        }
        
        [Theory]
        [ClassData(typeof(DeviceAndNotificationTestDataMultipleAccountId))]
        public async Task Handle_Verify_ExpectedMethodCalls(
            IEnumerable<DeviceRegistration> deviceRegistrations, 
            IEnumerable<string> enabledDevices,
            NotificationMessage notificationMessage)
        {
            var e = new MessagePreviewEvent() { Recipients = new[] { "A01" }, Subject = "some-subject", Content = "some-content" };
            
            var mockDeviceRegistrationService = new Mock<IDeviceRegistrationService>();

            //Setup the Mock (DeviceRegistrationService) so that it will return the given 'deviceRegistrations'.
            mockDeviceRegistrationService.Setup(x => x.GetDeviceRegistrationsAsync(It.IsAny<string[]>())).ReturnsAsync(
                new Result<IEnumerable<DeviceRegistration>, DeviceRegistrationErrorCode>(deviceRegistrations));
                
            var mockNotificationService = new Mock<INotificationService>();
            
            // Setup the mock (NotificationService) so that BuildNotificationMessage will return the given 'notificationMessage'
            mockNotificationService.Setup(x => x.BuildNotificationMessage(It.IsAny<NotificationType>(),
                It.IsAny<string>(), 
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, string>>())).Returns(notificationMessage);
            
            // Setup the mock (NotificationService) so that IsDeviceTargeted will return true when called with any device id that's specified in 'enabledDevices'
            mockNotificationService.Setup(x => x.IsDeviceTargeted(It.Is<DeviceConfiguration>(dc => enabledDevices.Contains(dc.DeviceId)), It.IsAny<NotificationType>())).Returns(true);
            mockNotificationService.Setup(x => x.IsDeviceTargeted(It.Is<DeviceConfiguration>(dc => !enabledDevices.Contains(dc.DeviceId)), It.IsAny<NotificationType>())).Returns(false);
            
            var deviceConfigurationRepositoryStub = new DeviceConfigurationRepositoryStub();

            var sut = CreateSut(notificationServiceArg: mockNotificationService.Object,
                 deviceRegistrationServiceArg: mockDeviceRegistrationService.Object,
                 deviceConfigurationRepositoryArg: deviceConfigurationRepositoryStub);
            
            await sut.Handle(e);
            
            // Verify that the mock (DeviceRegistrationService) was called with the correct parameters
            mockDeviceRegistrationService.Verify(x => x.GetDeviceRegistrationsAsync(It.Is<string[]>(ids => ids.Count() == e.Recipients.Count())), Times.Once);
            
            // Verify that the mock (NotificationService).IsDeviceTargeted was called with the correct parameters
            mockNotificationService.Verify(x => x.IsDeviceTargeted(It.IsAny<DeviceConfiguration>(), It.IsAny<NotificationType>()), Times.Exactly(deviceRegistrations.Count()));

            // Verify that it calls BuildNotificationMessage and SendNotificationMessage X times where X equals to enabledDevices.Count
            mockNotificationService.Verify(x => x.BuildNotificationMessage(It.IsAny<NotificationType>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()), Times.Exactly(enabledDevices.Count()));
            mockNotificationService.Verify(x => x.SendNotification(notificationMessage, It.IsAny<string>()), Times.Exactly(enabledDevices.Count()));

            // Verify that DeviceConfigurationRepository is called correct number of times.
            Assert.Equal(deviceConfigurationRepositoryStub.NumOfGetAsyncCalls, deviceRegistrations.Count());
        }

        #endregion
        
        #region BuildNotificationMessage

        [Fact]
        public async Task BuildNotificationType_InboxMessageMapping_HappyPath()
        {
            var e = new MessagePreviewEvent() { Subject = "some-subject", Content = "some-content" };
            
            var mockNotificationService = new Mock<INotificationService>();
            
            var sut = CreateSut(notificationServiceArg: mockNotificationService.Object);
            
            var message = await sut.BuildNotificationMessage(e, NotificationType.InboxMessage, Locale.En);
            
            mockNotificationService.Verify(x => x.BuildNotificationMessage(NotificationType.InboxMessage, e.Subject, e.Content, It.IsAny<Dictionary<string, string>>()), Times.Once);
        }
        
        [Fact]
        public async Task BuildNotificationMessage_ShouldntCallLocalizationService_IfItsAnInboxMessage()
        {
            var e = new MessagePreviewEvent() { Subject = "some-subject", Content = "some-content" };
            
            var mockLocalizationService = new Mock<ILocalizationService>();
            var sut = CreateSut(localizationServiceArg: mockLocalizationService.Object);
            
            await sut.BuildNotificationMessage(e, NotificationType.InboxMessage, Locale.En);
            
            mockLocalizationService.Verify(x => x.GetLocalizedTextAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IList<string>>()), Times.Never);
        }
        
        [Theory]
        [InlineData(NotificationType.MarketHoliday, Locale.En)]
        public async Task BuildNotificationMessage_HappyPath_IfItsNotInboxMessage(NotificationType notificationType, Locale locale)
        {
            var e = new MessagePreviewEvent() { LocalizationAttributes = new[] { "some-attribute" }};
            
            var localizedTitle = "localized-title";
            var localizedBody = "localized-body";

            var mockLocalizationService = new Mock<ILocalizationService>();
            mockLocalizationService.Setup(x => x.GetLocalizedTextAsync(Enum.GetName(notificationType), Enum.GetName(locale), e.LocalizationAttributes)).ReturnsAsync((localizedTitle, localizedBody));
            
            var mockNotificationService = new Mock<INotificationService>();
            
            var sut = CreateSut(localizationServiceArg: mockLocalizationService.Object, 
                notificationServiceArg: mockNotificationService.Object);
            
            await sut.BuildNotificationMessage(e, notificationType, locale);
            
            mockLocalizationService.Verify(x => x.GetLocalizedTextAsync(Enum.GetName(notificationType), Enum.GetName(locale), e.LocalizationAttributes), Times.Once);
            mockNotificationService.Verify(x => x.BuildNotificationMessage(notificationType, localizedTitle, localizedBody, It.IsAny<Dictionary<string, string>>()), Times.Once);
        }
        
        [Fact]
        public async Task BuildNotificationMessage_PassEmptyArray_IfLocalizationAttributesIsNull()
        {
            var e = new MessagePreviewEvent();
            
            var mockLocalizationService = new Mock<ILocalizationService>();
            
            var sut = CreateSut(localizationServiceArg: mockLocalizationService.Object);
            
            await sut.BuildNotificationMessage(e, NotificationType.PlatformHoliday, Locale.En);
            
            mockLocalizationService.Verify(x => x.GetLocalizedTextAsync(It.IsAny<string>(), It.IsAny<string>(), Array.Empty<string>()), Times.Once);
        }

        #endregion

        private MessagePreviewEventHandler CreateSut(INotificationService? notificationServiceArg = null,
            IDeviceRegistrationService? deviceRegistrationServiceArg = null,
            IDeviceConfigurationRepository? deviceConfigurationRepositoryArg = null,
            ILocalizationService? localizationServiceArg = null)
        {
            var mockLogger = new Mock<ILogger<MessagePreviewEventHandler>>();

            INotificationService notificationService = new Mock<INotificationService>().Object;
            IDeviceRegistrationService deviceRegistrationService = new Mock<IDeviceRegistrationService>().Object;
            IDeviceConfigurationRepository deviceConfigurationRepository = new Mock<IDeviceConfigurationRepository>().Object;
            ILocalizationService localizationService = new Mock<ILocalizationService>().Object;
            IReadOnlyDictionary<MessageEventType, NotificationType> notificationTypeMapping = new Dictionary<MessageEventType, NotificationType>()
            {
                { MessageEventType.Custom, NotificationType.InboxMessage },
                { MessageEventType.PositionsAboutToClose, NotificationType.CAPositionAboutToClose },
                { MessageEventType.PositionsAboutToClose_871m, NotificationType.CAPositionAboutToClose },
                { MessageEventType.MarketHoliday, NotificationType.MarketHoliday },
                { MessageEventType.PlatformHoliday, NotificationType.PlatformHoliday }
            };
            
            if(notificationServiceArg != null)
            {
                notificationService = notificationServiceArg;
            }
            
            if(deviceRegistrationServiceArg != null)
            {
                deviceRegistrationService = deviceRegistrationServiceArg;
            }
            
            if(deviceConfigurationRepositoryArg != null)
            {
                deviceConfigurationRepository = deviceConfigurationRepositoryArg;
            }
            
            if(localizationServiceArg != null)
            {
                localizationService = localizationServiceArg;
            }
            
            return new MessagePreviewEventHandler(mockLogger.Object,
                deviceRegistrationService, 
                notificationService,
                deviceConfigurationRepository,
                localizationService);
        }
    }
}
