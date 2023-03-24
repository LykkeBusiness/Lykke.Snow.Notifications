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
using Lykke.Snow.Notifications.Domain.Exceptions;
using Lykke.Snow.Notifications.Domain.Model;
using Lykke.Snow.Notifications.Domain.Repositories;
using Lykke.Snow.Notifications.Domain.Services;
using Microsoft.Extensions.Logging;

namespace Lykke.Snow.Notifications.DomainServices.Projections
{
    public class ActivityProjection
    {
        private Dictionary<ActivityTypeContract, NotificationType> _notificationTypeMapping = new Dictionary<ActivityTypeContract, NotificationType>()
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

        private readonly ILogger<ActivityProjection> _logger;

        private readonly INotificationService _notificationService;

        private readonly IDeviceRegistrationService _deviceRegistrationService;

        private readonly ILocalizationService _localizationService;
        
        private readonly IDeviceConfigurationRepository _deviceConfigurationRepository;

        public ActivityProjection(ILogger<ActivityProjection> logger,
            INotificationService notificationService,
            IDeviceRegistrationService deviceRegistrationService,
            ILocalizationService localizationService,
            IDeviceConfigurationRepository deviceConfigurationRepository)
        {
            _logger = logger;
            _notificationService = notificationService;
            _deviceRegistrationService = deviceRegistrationService;
            _localizationService = localizationService;
            _deviceConfigurationRepository = deviceConfigurationRepository;
        }

        [UsedImplicitly]
        public async Task Handle(ActivityEvent e)
        {
            _logger.LogInformation("A new activity event has just arrived {ActivityEvent}", e.ToJson());
            
            if(!TryGetNotificationType(activityEvent: e, out var notificationTypeStr, out var notificationType))
            {
                // We silently ignore if there's no notification type is defined for the activity.
                return;
            }

            if(notificationTypeStr == null)
                throw new NotificationTypeConversionException("could not get the NotificationType in string");
            
            var deviceRegistrationsResult = await _deviceRegistrationService.GetDeviceRegistrationsAsync(accountId: e.Activity.AccountId);
            
            if(deviceRegistrationsResult.IsFailed)
            {
                _logger.LogWarning("Could not get device tokens for the account {AccountId}", e.Activity.AccountId);
                return;
            }

            foreach(var deviceRegistration in deviceRegistrationsResult.Value)
            {
                try 
                {
                    var deviceConfiguration = await _deviceConfigurationRepository.GetAsync(deviceId: deviceRegistration.DeviceId);
                        
                    if(!Filter(deviceConfiguration, notificationTypeStr))
                        continue;

                    var (title, body) = _localizationService.GetLocalizedText(
                        notificationType: notificationTypeStr, 
                        language: deviceConfiguration.Locale, 
                        parameters: e.Activity.DescriptionAttributes);

                    var notificationMessage = new NotificationMessage(
                        title, 
                        body, 
                        notificationType, 
                        new Dictionary<string, string>());
                    
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
        
        private bool Filter(DeviceConfiguration deviceConfiguration, string notificationType)
        {
            var enabledNotificationTypes = deviceConfiguration.EnabledNotifications.Select(x => x.Type.ToString()).ToHashSet();
            
            return enabledNotificationTypes.Contains(notificationType);
        }
        
        private bool TryGetNotificationType(ActivityEvent activityEvent, out string? typeStr, out NotificationType type)
        {
            if(!_notificationTypeMapping.ContainsKey(activityEvent.Activity.Event))
            {
                typeStr = null;
                type = NotificationType.NotSpecified;
                return false;
            }
            
            type = _notificationTypeMapping[activityEvent.Activity.Event];

            typeStr = Enum.GetName(type);

            return true;
        }
    }
}
