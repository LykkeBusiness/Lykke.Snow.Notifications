using System.Collections;
using System.Collections.Generic;
using Lykke.MarginTrading.Activities.Contracts.Models;
using Lykke.Snow.Notifications.Domain.Enums;
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

            yield return new object[] { ActivityTypeContract.AccountDepositSucceeded, mapping, NotificationType.DepositSucceeded };
            yield return new object[] { ActivityTypeContract.AccountTradingDisabled, mapping, NotificationType.AccountLocked };
            yield return new object[] { ActivityTypeContract.AccountDepositFailed, mapping, NotificationType.DepositFailed };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class ActivityProjectionTests
    {
        [Theory]
        [ClassData(typeof(ActivityEventMappingTestData))]
        public void TryGetNotificationType_ShouldMapActivity_ToNotificationType(
            ActivityTypeContract activityType,
            IReadOnlyDictionary<ActivityTypeContract, NotificationType> mapping, 
            NotificationType expected)
        {
            ActivityProjection.TryGetNotificationType(mapping, activityType, out var result);
            
            Assert.Equal(expected, result);
        }
        
        [Fact]
        public void EnrichActivityDescriptions_ShouldEnrich_ActivityDescriptions()
        {
        }

        
        private ActivityProjection CreateSut(INotificationService notificationServiceArg = null,
            IDeviceRegistrationService deviceRegistrationServiceArg = null,
            ILocalizationService localizationServiceArg = null,
            IDeviceConfigurationRepository deviceConfigurationRepositoryArg = null)
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
