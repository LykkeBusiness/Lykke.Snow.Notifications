using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.MarginTrading.Activities.Contracts.Models;
using Lykke.Snow.Notifications.Domain.Enums;
using Lykke.Snow.Notifications.Domain.Model;
using Lykke.Snow.Notifications.Domain.Repositories;
using Lykke.Snow.Notifications.Domain.Services;
using Lykke.Snow.Notifications.DomainServices.Projections;
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
                { ActivityTypeContract.AccountDepositFailed, NotificationType.DepositFailed },
            };

            yield return new object[] { ActivityTypeContract.AccountDepositSucceeded, mapping, NotificationType.DepositSucceeded, true };
            yield return new object[] { ActivityTypeContract.AccountTradingDisabled, mapping, NotificationType.AccountLocked, true };
            yield return new object[] { ActivityTypeContract.AccountDepositFailed, mapping, NotificationType.DepositFailed, true };
            yield return new object[] { ActivityTypeContract.MarginCall1, mapping, NotificationType.NotSpecified, false };
            yield return new object[] { ActivityTypeContract.PositionClosing, mapping, NotificationType.NotSpecified, false };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class ActivityProjectionTests
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
            var result = ActivityProjection.TryGetNotificationType(mapping, activityType, out var notificationType);
            
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
                    descriptionAttributes: new string[] { "100", "EUR" }, 
                    new string[]{})
            };
            
            var result = ActivityProjection.EnrichActivityDescriptions(descriptionEnrichments, activity);
            
            
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
                    descriptionAttributes: new string[] { "100", "Buy", "Market", "Facebook Inc" }, 
                    new string[]{})
            };
            
            var result = ActivityProjection.EnrichActivityDescriptions(descriptionEnrichments, activity);
            
            
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
            
            var sut = CreateSut(deviceRegistrationServiceArg: deviceRegistrationServiceMock.Object);
            
            IReadOnlyDictionary<ActivityTypeContract, NotificationType> mapping = new Dictionary<ActivityTypeContract, NotificationType>()
            {
                { ActivityTypeContract.AccountDepositSucceeded, NotificationType.DepositSucceeded },
                { ActivityTypeContract.AccountTradingDisabled, NotificationType.AccountLocked },
                { ActivityTypeContract.AccountDepositFailed, NotificationType.DepositFailed },
            };

            var activity = new ActivityEvent
            {
                Activity = new ActivityContract("some-id", 
                    accountId: "some-account-id", 
                    instrument: "some-instrument", 
                    "some-event-source-id", 
                    DateTime.UtcNow, 
                    ActivityCategoryContract.Account,
                    ActivityTypeContract.SessionSwitchedToOnBehalfTrading, 
                    descriptionAttributes: new string[] { "100", "Buy", "Market", "Facebook Inc" }, 
                    new string[]{})
            };
            
            await sut.Handle(activity);
            
            deviceRegistrationServiceMock.Verify(x => x.GetDeviceRegistrationsAsync(It.IsAny<string>()), Times.Never);
            deviceConfigurationRepositoryMock.Verify(x => x.GetAsync(It.IsAny<string>()), Times.Never);
            notificationServiceMock.Verify(x => x.IsDeviceTargeted(It.IsAny<DeviceConfiguration>(), It.IsAny<NotificationType>()), Times.Never);
            notificationServiceMock.Verify(x => x.BuildLocalizedNotificationMessage(It.IsAny<NotificationType>(), It.IsAny<string[]>(), It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()), Times.Never);
            notificationServiceMock.Verify(x => x.SendNotification(It.IsAny<NotificationMessage>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task HandleActivityEvent_ShouldExitMethod_IfGetDeviceRegistrationHasFailed()
        {
            var deviceRegistrationServiceMock = new Mock<IDeviceRegistrationService>();
            deviceRegistrationServiceMock.Setup(x => x.GetDeviceRegistrationsAsync(It.IsAny<string>())).ReturnsAsync(new Common.Model.Result<IEnumerable<DeviceRegistration>, DeviceRegistrationErrorCode>(DeviceRegistrationErrorCode.DoesNotExist));
            
            var deviceConfigurationRepositoryMock = new Mock<IDeviceConfigurationRepository>();
            var notificationServiceMock = new Mock<INotificationService>();
            
            var sut = CreateSut(deviceRegistrationServiceArg: deviceRegistrationServiceMock.Object);
            
            IReadOnlyDictionary<ActivityTypeContract, NotificationType> mapping = new Dictionary<ActivityTypeContract, NotificationType>()
            {
                { ActivityTypeContract.AccountDepositSucceeded, NotificationType.DepositSucceeded },
                { ActivityTypeContract.AccountTradingDisabled, NotificationType.AccountLocked },
                { ActivityTypeContract.AccountDepositFailed, NotificationType.DepositFailed },
            };

            var activity = new ActivityEvent
            {
                Activity = new ActivityContract("some-id", 
                    accountId: "some-account-id", 
                    instrument: "some-instrument", 
                    "some-event-source-id", 
                    DateTime.UtcNow, 
                    ActivityCategoryContract.Account,
                    ActivityTypeContract.SessionSwitchedToOnBehalfTrading, 
                    descriptionAttributes: new string[] { "100", "Buy", "Market", "Facebook Inc" }, 
                    new string[]{})
            };
            
            await sut.Handle(activity);
            
            deviceConfigurationRepositoryMock.Verify(x => x.GetAsync(It.IsAny<string>()), Times.Never);
            notificationServiceMock.Verify(x => x.IsDeviceTargeted(It.IsAny<DeviceConfiguration>(), It.IsAny<NotificationType>()), Times.Never);
            notificationServiceMock.Verify(x => x.BuildLocalizedNotificationMessage(It.IsAny<NotificationType>(), It.IsAny<string[]>(), It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()), Times.Never);
            notificationServiceMock.Verify(x => x.SendNotification(It.IsAny<NotificationMessage>(), It.IsAny<string>()), Times.Never);
        }

        #endregion

        private ActivityProjection CreateSut(INotificationService? notificationServiceArg = null,
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
            
            return new ActivityProjection(mockLogger.Object, notificationService, deviceRegistrationService, localizationService, deviceConfigurationRepository);
        }
    }
}
