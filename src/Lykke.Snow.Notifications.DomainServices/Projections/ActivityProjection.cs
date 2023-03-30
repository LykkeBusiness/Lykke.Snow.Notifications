using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using FirebaseAdmin.Messaging;
using JetBrains.Annotations;
using Lykke.MarginTrading.Activities.Contracts.Models;
using Lykke.Snow.FirebaseIntegration.Exceptions;
using Lykke.Snow.Notifications.Domain.Enums;
using Lykke.Snow.Notifications.Domain.Repositories;
using Lykke.Snow.Notifications.Domain.Services;
using Microsoft.Extensions.Logging;

namespace Lykke.Snow.Notifications.DomainServices.Projections
{
    public class ActivityProjection
    {
        private IReadOnlyDictionary<ActivityTypeContract, NotificationType> _notificationTypeMapping = new Dictionary<ActivityTypeContract, NotificationType>()
        {
            { ActivityTypeContract.AccountTradingDisabled, NotificationType.AccountLocked },
            { ActivityTypeContract.AccountTradingEnabled, NotificationType.AccountUnlocked },
            { ActivityTypeContract.AccountDepositSucceeded, NotificationType.DepositSucceeded },
            { ActivityTypeContract.AccountDepositFailed, NotificationType.DepositFailed },
            { ActivityTypeContract.AccountWithdrawalSucceeded, NotificationType.WithdrawalSucceeded },
            { ActivityTypeContract.AccountWithdrawalFailed, NotificationType.WithdrawalFailed },
            { ActivityTypeContract.AccountWithdrawalDisabled, NotificationType.CashLocked },
            { ActivityTypeContract.Liquidation, NotificationType.Liquidation },
            { ActivityTypeContract.MarginCall1, NotificationType.MarginCall1 },
            { ActivityTypeContract.MarginCall2, NotificationType.MarginCall2 },
            { ActivityTypeContract.OrderExecution, NotificationType.OrderExecuted },
            { ActivityTypeContract.OrderAcceptanceAndExecution, NotificationType.OrderExecuted },
            { ActivityTypeContract.OrderExpiry, NotificationType.OrderExpired },
            { ActivityTypeContract.PositionClosing, NotificationType.PositionClosed },
            { ActivityTypeContract.PositionPartialClosing, NotificationType.PositionClosed }
        };

        private IReadOnlyDictionary<ActivityTypeContract, Func<ActivityEvent, string[]>> descriptionEnrichments = 
            new Dictionary<ActivityTypeContract, Func<ActivityEvent, string[]>>
        {
            {ActivityTypeContract.AccountWithdrawalSucceeded, (e) => { return e.Activity.DescriptionAttributes.ToList().Append(e.Activity.AccountId).ToArray(); }},
            {ActivityTypeContract.AccountDepositSucceeded, (e) => { return e.Activity.DescriptionAttributes.ToList().Append(e.Activity.AccountId).ToArray(); }}
        };

        private readonly ILogger<ActivityProjection> _logger;

        private readonly INotificationService _notificationService;

        private readonly IDeviceRegistrationService _deviceRegistrationService;

        private readonly IDeviceConfigurationRepository _deviceConfigurationRepository;

        private readonly ILocalizationService _localizationService;

        public ActivityProjection(ILogger<ActivityProjection> logger,
            INotificationService notificationService,
            IDeviceRegistrationService deviceRegistrationService,
            ILocalizationService localizationService,
            IDeviceConfigurationRepository deviceConfigurationRepository)
        {
            _logger = logger;
            _notificationService = notificationService;
            _deviceRegistrationService = deviceRegistrationService;
            _deviceConfigurationRepository = deviceConfigurationRepository;
            _localizationService = localizationService;
        }

        [UsedImplicitly]
        public async Task Handle(ActivityEvent e)
        {
            if(!TryGetNotificationType(_notificationTypeMapping, activityType: e.Activity.Event, out var notificationType))
                return;

            var deviceRegistrationsResult = await _deviceRegistrationService.GetDeviceRegistrationsAsync(accountId: e.Activity.AccountId);
            
            if(deviceRegistrationsResult.IsFailed)
            {
                _logger.LogWarning("Could not get device tokens for the account {AccountId}. ErrorCode: {ErrorCode}", e.Activity.AccountId, deviceRegistrationsResult.Error);
                return;
            }
            
            // Not all activities have enough number of description attributes
            // to fill in localization template. Here we enrich them.
            var notificationArguments = EnrichActivityDescriptions(descriptionEnrichments, e);

            foreach(var deviceRegistration in deviceRegistrationsResult.Value)
            {
                try 
                {
                    var deviceConfiguration = await _deviceConfigurationRepository.GetAsync(deviceId: deviceRegistration.DeviceId);
                        
                    if(!_notificationService.IsDeviceTargeted(deviceConfiguration, notificationType))
                        continue;
                        
                    var (title, body) = await _localizationService.GetLocalizedTextAsync(
                        Enum.GetName(notificationType), 
                        Enum.GetName(deviceConfiguration.Locale), 
                        notificationArguments);
                
                    var notificationMessage = _notificationService.BuildNotificationMessage(
                        notificationType,
                        title,
                        body,
                        new Dictionary<string, string>()
                    );

                    await _notificationService.SendNotification(notificationMessage, deviceToken: deviceRegistration.DeviceToken);

                    _logger.LogInformation("Push notification has successfully been sent to the device {DeviceToken}: {PushNotificationPayload}",
                        deviceRegistration.DeviceToken, notificationMessage.ToJson());
                }
                catch(CannotSendNotificationException ex)
                {
                    if(ex.ErrorCode == MessagingErrorCode.Unregistered)
                    {
                        _logger.LogWarning("The notification could not be delivered to the device {DeviceToken} because it is no longer active.", deviceRegistration.DeviceToken);
                    }
                }
            }
        }
        
        public static bool TryGetNotificationType(IReadOnlyDictionary<ActivityTypeContract, NotificationType> notificationTypeMapping, ActivityTypeContract activityType, out NotificationType type)
        {
            if(!notificationTypeMapping.ContainsKey(activityType))
            {
                type = NotificationType.NotSpecified;
                return false;
            }
            
            type = notificationTypeMapping[activityType];

            return true;
        }
        
        public static string[] EnrichActivityDescriptions(IReadOnlyDictionary<ActivityTypeContract, Func<ActivityEvent, string[]>> enrichments, ActivityEvent e)
        {
            if(enrichments.ContainsKey(e.Activity.Event))
               return enrichments[e.Activity.Event](e);
            
            return e.Activity.DescriptionAttributes;
        }
    }
}
