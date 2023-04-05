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

            var sut = CreateSut(notificationServiceArg: mockNotificationService.Object,
                deviceRegistrationServiceArg: mockDeviceRegistrationService.Object,
                deviceConfigurationRepositoryArg: mockDeviceConfigurationRepository.Object);
            
            var e = new MessagePreviewEvent();
            
            await sut.Handle(e);
            
            mockDeviceRegistrationService.Verify(x => x.GetDeviceRegistrationsAsync(It.IsAny<string[]>()), Times.Never);
            mockNotificationService.Verify(x => x.BuildNotificationMessage(It.IsAny<NotificationType>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()), Times.Never);
            mockDeviceConfigurationRepository.Verify(x => x.GetAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            mockNotificationService.Verify(x => x.IsDeviceTargeted(It.IsAny<DeviceConfiguration>(), It.IsAny<NotificationType>()), Times.Never);
            mockNotificationService.Verify(x => x.IsDeviceTargeted(It.IsAny<DeviceConfiguration>(), It.IsAny<NotificationType>()), Times.Never);
            mockNotificationService.Verify(x => x.SendNotification(It.IsAny<NotificationMessage>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldExitMethod_IfSubjectOrContentIsNull()
        {
            var mockDeviceRegistrationService = new Mock<IDeviceRegistrationService>();
            var mockNotificationService = new Mock<INotificationService>();
            var mockDeviceConfigurationRepository = new Mock<IDeviceConfigurationRepository>();

            var sut = CreateSut(notificationServiceArg: mockNotificationService.Object,
                deviceRegistrationServiceArg: mockDeviceRegistrationService.Object,
                deviceConfigurationRepositoryArg: mockDeviceConfigurationRepository.Object);
            
            var e = new MessagePreviewEvent() 
            {
                Recipients = new List<string> { "account-id-1" }
            };
            
            await sut.Handle(e);
            
            mockDeviceRegistrationService.Verify(x => x.GetDeviceRegistrationsAsync(It.IsAny<string[]>()), Times.Never);
            mockNotificationService.Verify(x => x.BuildNotificationMessage(It.IsAny<NotificationType>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()), Times.Never);
            mockDeviceConfigurationRepository.Verify(x => x.GetAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            mockNotificationService.Verify(x => x.IsDeviceTargeted(It.IsAny<DeviceConfiguration>(), It.IsAny<NotificationType>()), Times.Never);
            mockNotificationService.Verify(x => x.IsDeviceTargeted(It.IsAny<DeviceConfiguration>(), It.IsAny<NotificationType>()), Times.Never);
            mockNotificationService.Verify(x => x.SendNotification(It.IsAny<NotificationMessage>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldExitMethod_IfEventIsNull()
        {
            var mockDeviceRegistrationService = new Mock<IDeviceRegistrationService>();
            var mockNotificationService = new Mock<INotificationService>();
            var mockDeviceConfigurationRepository = new Mock<IDeviceConfigurationRepository>();

            var sut = CreateSut(notificationServiceArg: mockNotificationService.Object,
                deviceRegistrationServiceArg: mockDeviceRegistrationService.Object,
                deviceConfigurationRepositoryArg: mockDeviceConfigurationRepository.Object);
            
            await sut.Handle(null);
            
            mockDeviceRegistrationService.Verify(x => x.GetDeviceRegistrationsAsync(It.IsAny<string[]>()), Times.Never);
            mockNotificationService.Verify(x => x.BuildNotificationMessage(It.IsAny<NotificationType>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()), Times.Never);
            mockDeviceConfigurationRepository.Verify(x => x.GetAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            mockNotificationService.Verify(x => x.IsDeviceTargeted(It.IsAny<DeviceConfiguration>(), It.IsAny<NotificationType>()), Times.Never);
            mockNotificationService.Verify(x => x.IsDeviceTargeted(It.IsAny<DeviceConfiguration>(), It.IsAny<NotificationType>()), Times.Never);
            mockNotificationService.Verify(x => x.SendNotification(It.IsAny<NotificationMessage>(), It.IsAny<string>()), Times.Never);
        }
        
        [Fact]
        public void Handle_ShouldExitMethod_IfGetDeviceRegistrationHasFailed()
        {
            var mockDeviceRegistrationService = new Mock<IDeviceRegistrationService>();
            var mockNotificationService = new Mock<INotificationService>();
            var mockDeviceConfigurationRepository = new Mock<IDeviceConfigurationRepository>();
            mockDeviceRegistrationService.Setup(x => x.GetDeviceRegistrationsAsync(It.IsAny<string>())).ReturnsAsync(new Result<IEnumerable<DeviceRegistration>, DeviceRegistrationErrorCode>(DeviceRegistrationErrorCode.DoesNotExist));

            mockNotificationService.Verify(x => x.BuildNotificationMessage(It.IsAny<NotificationType>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()), Times.Never);
            mockDeviceConfigurationRepository.Verify(x => x.GetAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            mockNotificationService.Verify(x => x.IsDeviceTargeted(It.IsAny<DeviceConfiguration>(), It.IsAny<NotificationType>()), Times.Never);
            mockNotificationService.Verify(x => x.IsDeviceTargeted(It.IsAny<DeviceConfiguration>(), It.IsAny<NotificationType>()), Times.Never);
            mockNotificationService.Verify(x => x.SendNotification(It.IsAny<NotificationMessage>(), It.IsAny<string>()), Times.Never);
        }

        [Theory]
        [ClassData(typeof(DeviceRegistrationsTestData))]
        public async Task Handle_ShouldExitMethod_IfDeviceConfigurationWasNotFound(List<DeviceRegistration> deviceRegistrations)
        {
            var mockDeviceRegistrationService = new Mock<IDeviceRegistrationService>();
            mockDeviceRegistrationService.Setup(x => x.GetDeviceRegistrationsAsync(It.IsAny<string>())).ReturnsAsync(deviceRegistrations);

            var mockNotificationService = new Mock<INotificationService>();
            var mockDeviceConfigurationRepository = new Mock<IDeviceConfigurationRepository>();
            mockDeviceConfigurationRepository.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult<DeviceConfiguration>(null));

            var sut = CreateSut(notificationServiceArg: mockNotificationService.Object,
                deviceRegistrationServiceArg: mockDeviceRegistrationService.Object,
                deviceConfigurationRepositoryArg: mockDeviceConfigurationRepository.Object);

            var e = new MessagePreviewEvent() 
            {
                Recipients = new List<string> { "account-id-1" }
            };
            
            await sut.Handle(e);

            mockNotificationService.Verify(x => x.BuildNotificationMessage(It.IsAny<NotificationType>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()), Times.Never);
            mockDeviceConfigurationRepository.Verify(x => x.GetAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            mockNotificationService.Verify(x => x.IsDeviceTargeted(It.IsAny<DeviceConfiguration>(), It.IsAny<NotificationType>()), Times.Never);
            mockNotificationService.Verify(x => x.IsDeviceTargeted(It.IsAny<DeviceConfiguration>(), It.IsAny<NotificationType>()), Times.Never);
            mockNotificationService.Verify(x => x.SendNotification(It.IsAny<NotificationMessage>(), It.IsAny<string>()), Times.Never);
        }

        [Theory]
        [ClassData(typeof(DeviceAndNotificationTestDataMultipleAccountId))]
        public async Task ProcessMessageAsync_Verify_ExpectedMethodCalls(
            IEnumerable<DeviceRegistration> deviceRegistrations, 
            IEnumerable<string> enabledDevices,
            NotificationMessage notificationMessage)
        {
            var e = new MessagePreviewEvent() { Recipients = new[] { "A01" }, Subject = "some-sobject", Content = "some-content" };
            
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
            
            mockDeviceRegistrationService.Verify(x => x.GetDeviceRegistrationsAsync(It.Is<string[]>(ids => ids.Count() == e.Recipients.Count())), Times.Once);
            mockNotificationService.Verify(x => x.IsDeviceTargeted(It.IsAny<DeviceConfiguration>(), It.IsAny<NotificationType>()), Times.Exactly(deviceRegistrations.Count()));
            mockNotificationService.Verify(x => x.SendNotification(notificationMessage, It.IsAny<string>()), Times.Exactly(enabledDevices.Count()));

            Assert.Equal(deviceConfigurationRepositoryStub.NumOfGetAsyncCalls, deviceRegistrations.Count());
        }

        #endregion

        private MessagePreviewEventHandler CreateSut(INotificationService? notificationServiceArg = null,
            IDeviceRegistrationService? deviceRegistrationServiceArg = null,
            IDeviceConfigurationRepository? deviceConfigurationRepositoryArg = null)
        {

            var mockLogger = new Mock<ILogger<MessagePreviewEventHandler>>();

            INotificationService notificationService = new Mock<INotificationService>().Object;
            IDeviceRegistrationService deviceRegistrationService = new Mock<IDeviceRegistrationService>().Object;
            IDeviceConfigurationRepository deviceConfigurationRepository = new Mock<IDeviceConfigurationRepository>().Object;
            
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
            
            return new MessagePreviewEventHandler(mockLogger.Object,
                deviceRegistrationService, 
                notificationService,
                deviceConfigurationRepository);
        }
    }
}
