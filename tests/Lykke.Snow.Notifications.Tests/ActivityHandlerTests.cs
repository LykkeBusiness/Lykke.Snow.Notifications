using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.MarginTrading.Activities.Contracts.Models;
using Lykke.Snow.Common.Model;
using Lykke.Snow.Notifications.Domain.Enums;
using Lykke.Snow.Notifications.Domain.Model;
using Lykke.Snow.Notifications.Domain.Repositories;
using Lykke.Snow.Notifications.Domain.Services;
using Lykke.Snow.Notifications.DomainServices.Projections;
using Lykke.Snow.Notifications.DomainServices.Services;
using Lykke.Snow.Notifications.Tests.Fakes;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Lykke.Snow.Notifications.Tests
{
    class ActivityEventMappingTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            IReadOnlyDictionary<ActivityTypeContract, NotificationType> mapping = new Dictionary<ActivityTypeContract, NotificationType>()
            {
                { ActivityTypeContract.AccountDepositSucceeded, NotificationType.DepositSucceeded },
                { ActivityTypeContract.AccountTradingDisabled, NotificationType.AccountLocked },
                { ActivityTypeContract.Liquidation, NotificationType.Liquidation },
            };

            yield return new object[] { ActivityTypeContract.AccountDepositSucceeded, mapping, NotificationType.DepositSucceeded, true };
            yield return new object[] { ActivityTypeContract.AccountTradingDisabled, mapping, NotificationType.AccountLocked, true };
            yield return new object[] { ActivityTypeContract.Liquidation, mapping, NotificationType.Liquidation, true };
            yield return new object[] { ActivityTypeContract.MarginCall1, mapping, NotificationType.NotSpecified, false };
            yield return new object[] { ActivityTypeContract.PositionClosing, mapping, NotificationType.NotSpecified, false };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    class DeviceAndNotificationTestDataSingleAccountId : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] 
            { 
                "account-id-1",
                new List<DeviceRegistration> 
                {
                    new DeviceRegistration("account-id-1", "device-token-1", "device1", DateTime.UtcNow),
                    new DeviceRegistration("account-id-1", "device-token-2", "device2", DateTime.UtcNow),
                    new DeviceRegistration("account-id-1", "device-token-3", "device3", DateTime.UtcNow),
                    new DeviceRegistration("account-id-1", "device-token-4", "device4", DateTime.UtcNow),
                    new DeviceRegistration("account-id-1", "device-token-5", "device5", DateTime.UtcNow),
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

    class DeviceRegistrationsTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] 
            { 
                new List<DeviceRegistration> 
                {
                    new DeviceRegistration("account-id-1", "device-token-3", "device3", DateTime.UtcNow),
                    new DeviceRegistration("account-id-1", "device-token-4", "device4", DateTime.UtcNow),
                    new DeviceRegistration("account-id-1", "device-token-5", "device5", DateTime.UtcNow),
                },
            };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    class DeviceRegistrationsWithMissingConfigurationTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] 
            { 
                new List<DeviceRegistration> 
                {
                    new DeviceRegistration("account-id-1", "device-token-3", "device3", DateTime.UtcNow),
                    new DeviceRegistration("account-id-1", "device-token-4", "device4", DateTime.UtcNow),
                    new DeviceRegistration("account-id-1", "device-token-5", "device5", DateTime.UtcNow),
                },
                new List<string>() { "device4" }
            };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }


    public class ActivityHandlerTests
    {
        #region Notification type mapping
        [Theory]
        [ClassData(typeof(ActivityEventMappingTestData))]
        public void TryGetNotificationType_ShouldMapActivity_ToNotificationType(
            ActivityTypeContract activityType,
            IReadOnlyDictionary<ActivityTypeContract, NotificationType> mapping, 
            NotificationType expectedNotificationType,
            bool expectedResult)
        {
            var result = ActivityHandler.TryGetNotificationType(mapping, activityType, out var notificationType);
            
            Assert.Equal(expectedNotificationType, notificationType);
            Assert.Equal(expectedResult, result);
        }
        #endregion
        
        #region Enrichments
        [Fact]
        public void EnrichActivityDescriptions_ShouldEnrich_ActivityDescriptions()
        {
            IReadOnlyDictionary<ActivityTypeContract, Func<ActivityEvent, string[]>> descriptionEnrichments = 
            new Dictionary<ActivityTypeContract, Func<ActivityEvent, string[]>>
            {
                { ActivityTypeContract.OrderAcceptance, 
                    (e) => { 
                                return e.Activity.DescriptionAttributes.ToList().Append(e.Activity.AccountId).Append(e.Activity.Instrument).ToArray(); 
                            }},
            };
            
            var activity = new ActivityEvent
            {
                Activity = new ActivityContract("some-id", 
                    accountId: "some-account-id", 
                    instrument: "some-instrument", 
                    "some-event-source-id", 
                    DateTime.UtcNow, 
                    ActivityCategoryContract.Account,
                    ActivityTypeContract.OrderAcceptance, 
                    descriptionAttributes: new[] { "100", "EUR" }, 
                    new string[]{})
            };
            
            var result = ActivityHandler.EnrichActivityDescriptions(descriptionEnrichments, activity);
            
            
            Assert.Equal(4, result.Count());
            Assert.Equal(activity.Activity.AccountId, result[2]);
            Assert.Equal(activity.Activity.Instrument, result[3]);
        }

        [Fact]
        public void EnrichActivityDescriptions_ShouldNotModify_ActivitiesThatDoesntHaveEnrichments()
        {
            IReadOnlyDictionary<ActivityTypeContract, Func<ActivityEvent, string[]>> descriptionEnrichments = 
            new Dictionary<ActivityTypeContract, Func<ActivityEvent, string[]>>
            {
                { ActivityTypeContract.OrderAcceptance, 
                    (e) => { 
                                return e.Activity.DescriptionAttributes.ToList().Append(e.Activity.AccountId).Append(e.Activity.Instrument).ToArray(); 
                            }},
            };

            var activity = new ActivityEvent
            {
                Activity = new ActivityContract("some-id", 
                    accountId: "some-account-id", 
                    instrument: "some-instrument", 
                    "some-event-source-id", 
                    DateTime.UtcNow, 
                    ActivityCategoryContract.Account,
                    ActivityTypeContract.OrderExecution, 
                    descriptionAttributes: new[] { "100", "Buy", "Market", "Facebook Inc" }, 
                    new string[]{})
            };
            
            var result = ActivityHandler.EnrichActivityDescriptions(descriptionEnrichments, activity);
            
            
            Assert.Equal(activity.Activity.DescriptionAttributes, result);
            Assert.Equal(4, result.Count());
        }
        #endregion
        
        
        #region Execution path tests
        
        [Fact]
        public async Task HandleActivityEvent_ShouldExitMethod_IfNotificationIsNotMapped()
        {
            var deviceRegistrationServiceMock = new Mock<IDeviceRegistrationService>();
            var deviceConfigurationRepositoryMock = new Mock<IDeviceConfigurationRepository>();
            var notificationServiceMock = new Mock<INotificationService>();
            var localizationServiceMock = new Mock<ILocalizationService>();
            
            var sut = CreateSut(deviceRegistrationServiceArg: deviceRegistrationServiceMock.Object, 
                localizationServiceArg: localizationServiceMock.Object);

            var activity = new ActivityEvent
            {
                Activity = new ActivityContract("some-id", 
                    accountId: "some-account-id", 
                    instrument: "some-instrument", 
                    "some-event-source-id", 
                    DateTime.UtcNow, 
                    ActivityCategoryContract.Account,
                    ActivityTypeContract.SessionSwitchedToOnBehalfTrading, 
                    descriptionAttributes: new[] { "100", "Buy", "Market", "Facebook Inc" }, 
                    new string[]{})
            };
            
            await sut.Handle(activity);
            
            deviceRegistrationServiceMock.Verify(x => x.GetDeviceRegistrationsAsync(It.IsAny<string>()), Times.Never);
            deviceConfigurationRepositoryMock.Verify(x => x.GetAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            notificationServiceMock.Verify(x => x.IsDeviceTargeted(It.IsAny<DeviceConfiguration>(), It.IsAny<NotificationType>()), Times.Never);
            notificationServiceMock.Verify(x => x.BuildNotificationMessage(It.IsAny<NotificationType>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()), Times.Never);
            notificationServiceMock.Verify(x => x.SendNotification(It.IsAny<NotificationMessage>(), It.IsAny<string>()), Times.Never);
            localizationServiceMock.Verify(x => x.GetLocalizedTextAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IReadOnlyList<string>>()), Times.Never);
        }

        [Fact]
        public async Task HandleActivityEvent_ShouldExitMethod_IfGetDeviceRegistrationHasFailed()
        {
            var deviceRegistrationServiceMock = new Mock<IDeviceRegistrationService>();
            deviceRegistrationServiceMock.Setup(x => x.GetDeviceRegistrationsAsync(It.IsAny<string>())).ReturnsAsync(new Result<IEnumerable<DeviceRegistration>, DeviceRegistrationErrorCode>(DeviceRegistrationErrorCode.DoesNotExist));
            
            var deviceConfigurationRepositoryMock = new Mock<IDeviceConfigurationRepository>();
            var notificationServiceMock = new Mock<INotificationService>();
            var localizationServiceMock = new Mock<ILocalizationService>();
            
            var sut = CreateSut(deviceRegistrationServiceArg: deviceRegistrationServiceMock.Object, 
                localizationServiceArg: localizationServiceMock.Object);

            var activity = new ActivityEvent
            {
                Activity = new ActivityContract("some-id", 
                    accountId: "some-account-id", 
                    instrument: "some-instrument", 
                    "some-event-source-id", 
                    DateTime.UtcNow, 
                    ActivityCategoryContract.Account,
                    ActivityTypeContract.OrderAcceptance, 
                    descriptionAttributes: new[] { "100", "Buy", "Market", "Facebook Inc" }, 
                    new string[]{})
            };
            
            await sut.Handle(activity);
            
            deviceConfigurationRepositoryMock.Verify(x => x.GetAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            notificationServiceMock.Verify(x => x.IsDeviceTargeted(It.IsAny<DeviceConfiguration>(), It.IsAny<NotificationType>()), Times.Never);
            notificationServiceMock.Verify(x => x.BuildNotificationMessage(It.IsAny<NotificationType>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()), Times.Never);
            notificationServiceMock.Verify(x => x.SendNotification(It.IsAny<NotificationMessage>(), It.IsAny<string>()), Times.Never);
            localizationServiceMock.Verify(x => x.GetLocalizedTextAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IReadOnlyList<string>>()), Times.Never);
        }

        [Theory]
        [ClassData(typeof(DeviceRegistrationsWithMissingConfigurationTestData))]
        public async Task HandleActivityEvent_ShouldSkipSending_IfDeviceConfigurationIsNotFound(
            List<DeviceRegistration> deviceRegistrations,
            List<string> deviceIdsMissingConfiguration)
        {
            var deviceRegistrationServiceMock = new Mock<IDeviceRegistrationService>();
            deviceRegistrationServiceMock.Setup(x => x.GetDeviceRegistrationsAsync(It.IsAny<string>())).ReturnsAsync(deviceRegistrations);
            
            var deviceConfigurationRepositoryMock = new Mock<IDeviceConfigurationRepository>();
            deviceConfigurationRepositoryMock.Setup(x => x.GetAsync(It.Is<string>(x => deviceIdsMissingConfiguration.Contains(x)), It.IsAny<string>()))
                .Returns(Task.FromResult<DeviceConfiguration>(null));

            deviceConfigurationRepositoryMock.Setup(x => x.GetAsync(It.Is<string>(x => !deviceIdsMissingConfiguration.Contains(x)), It.IsAny<string>()))
                .Returns(Task.FromResult<DeviceConfiguration>(new DeviceConfiguration("device-id", "account-id")));
            
            var notificationServiceMock = new Mock<INotificationService>();
            notificationServiceMock.Setup(x => x.IsDeviceTargeted(It.IsAny<DeviceConfiguration>(), It.IsAny<NotificationType>())).Returns(true);

            var localizationServiceMock = new Mock<ILocalizationService>();
            
            var sut = CreateSut(
                notificationServiceArg: notificationServiceMock.Object,
                deviceRegistrationServiceArg: deviceRegistrationServiceMock.Object, 
                localizationServiceArg: localizationServiceMock.Object,
                deviceConfigurationRepositoryArg: deviceConfigurationRepositoryMock.Object);

            var activity = new ActivityEvent
            {
                Activity = new ActivityContract("some-id", 
                    accountId: "some-account-id", 
                    instrument: "some-instrument", 
                    "some-event-source-id", 
                    DateTime.UtcNow, 
                    ActivityCategoryContract.Account,
                    ActivityTypeContract.PositionClosing, 
                    descriptionAttributes: new[] { "100", "Buy", "Market", "Facebook Inc" }, 
                    new string[]{})
            };
            
            await sut.Handle(activity);
            
            var activeDeviceCount = deviceRegistrations.Count - deviceIdsMissingConfiguration.Count;

            deviceConfigurationRepositoryMock.Verify(x => x.GetAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(deviceRegistrations.Count));
            notificationServiceMock.Verify(x => x.IsDeviceTargeted(It.IsAny<DeviceConfiguration>(), It.IsAny<NotificationType>()), Times.Exactly(activeDeviceCount));
            notificationServiceMock.Verify(x => x.BuildNotificationMessage(It.IsAny<NotificationType>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()), Times.Exactly(activeDeviceCount));
            notificationServiceMock.Verify(x => x.SendNotification(It.IsAny<NotificationMessage>(), It.IsAny<string>()), Times.Exactly(activeDeviceCount));
            localizationServiceMock.Verify(x => x.GetLocalizedTextAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IReadOnlyList<string>>()), Times.Exactly(activeDeviceCount));
        }

        [Theory]
        [ClassData(typeof(DeviceAndNotificationTestDataSingleAccountId))]
        public async void Handle_Verify_ExpectedMethodCalls(
            string accountId,
            IEnumerable<DeviceRegistration> deviceRegistrations,
            IEnumerable<string> enabledDevices,
            NotificationMessage notificationMessage
        )
        {
            var e = new ActivityEvent
            {
                Id = "some-id",
                Timestamp = DateTime.UtcNow,
                Activity = new ActivityContract("id", accountId, "instrument", "event-source-id", DateTime.UtcNow, ActivityCategoryContract.CashMovement,
                    ActivityTypeContract.OrderExpiry, new[] { "attr-1", "attr-2"}, new[] { "relatedid-1", "relatedid-2" })
            };

            var mockDeviceRegistrationService = new Mock<IDeviceRegistrationService>();

            //Setup the Mock (DeviceRegistrationService) so that it will return the given 'deviceRegistrations'.
            mockDeviceRegistrationService.Setup(x => x.GetDeviceRegistrationsAsync(It.IsAny<string>())).ReturnsAsync(
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
            var localizationServiceStub = new LocalizationServiceStub();
            
            var sut = CreateSut(notificationServiceArg: mockNotificationService.Object,
                deviceRegistrationServiceArg: mockDeviceRegistrationService.Object,
                deviceConfigurationRepositoryArg: deviceConfigurationRepositoryStub,
                localizationServiceArg: localizationServiceStub);
            
            await sut.Handle(e);
            
            Assert.Equal(deviceConfigurationRepositoryStub.NumOfGetAsyncCalls, deviceRegistrations.Count());
            Assert.Equal(localizationServiceStub.NumOfGetLocalizedTextAsyncCalls, enabledDevices.Count());
            mockDeviceRegistrationService.Verify(x => x.GetDeviceRegistrationsAsync(It.Is<string>(id => id == accountId)), Times.Once);
            mockNotificationService.Verify(x => x.IsDeviceTargeted(It.IsAny<DeviceConfiguration>(), It.IsAny<NotificationType>()), Times.Exactly(deviceRegistrations.Count()));
            mockNotificationService.Verify(x => x.BuildNotificationMessage(It.IsAny<NotificationType>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()), Times.Exactly(enabledDevices.Count()));
            mockNotificationService.Verify(x => x.SendNotification(notificationMessage, It.IsAny<string>()), Times.Exactly(enabledDevices.Count()));
        }
        #endregion

        private ActivityHandler CreateSut(INotificationService? notificationServiceArg = null,
            IDeviceRegistrationService? deviceRegistrationServiceArg = null,
            ILocalizationService? localizationServiceArg = null,
            IDeviceConfigurationRepository? deviceConfigurationRepositoryArg = null)
        {
            var mockLogger = new Mock<ILogger<ActivityProjection>>();

            INotificationService notificationService = new Mock<INotificationService>().Object;
            IDeviceRegistrationService deviceRegistrationService = new Mock<IDeviceRegistrationService>().Object;
            ILocalizationService localizationService = new Mock<ILocalizationService>().Object;
            IDeviceConfigurationRepository deviceConfigurationRepository = new Mock<IDeviceConfigurationRepository>().Object;
            
            if(notificationServiceArg != null)
            {
                notificationService = notificationServiceArg;
            }
            
            if(deviceRegistrationServiceArg != null)
            {
                deviceRegistrationService = deviceRegistrationServiceArg;
            }
            
            if(localizationServiceArg != null)
            {
                localizationService = localizationServiceArg;
            }
            
            if(deviceConfigurationRepositoryArg != null)
            {
                deviceConfigurationRepository = deviceConfigurationRepositoryArg;
            }
            
            return new ActivityHandler(mockLogger.Object, notificationService, deviceRegistrationService, localizationService, deviceConfigurationRepository);
        }
    }
}
