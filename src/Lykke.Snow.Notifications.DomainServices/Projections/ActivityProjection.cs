using System.Collections.Generic;
using System.Threading.Tasks;
using Common;
using FirebaseAdmin.Messaging;
using JetBrains.Annotations;
using Lykke.MarginTrading.Activities.Contracts.Models;
using Lykke.Snow.FirebaseIntegration.Exceptions;
using Lykke.Snow.Notifications.Domain.Enums;
using Lykke.Snow.Notifications.Domain.Model;
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

        public ActivityProjection(ILogger<ActivityProjection> logger,
            INotificationService notificationService,
            IDeviceRegistrationService deviceRegistrationService,
            ILocalizationService localizationService)
        {
            _logger = logger;
            _notificationService = notificationService;
            _deviceRegistrationService = deviceRegistrationService;
            _localizationService = localizationService;
        }

        [UsedImplicitly]
        public async Task Handle(ActivityEvent e)
        {
            _logger.LogInformation("A new activity event has just arrived {ActivityEvent}", e.ToJson());

            
            NotificationType? notificationType;
            
            if(!TryGetNotificationType(activityEvent: e, out notificationType))
                return;
            
            if(notificationType == null)
                return;
            
            var deviceRegistrationsResult = await _deviceRegistrationService.GetDeviceRegistrationsAsync(accountId: e.Activity.AccountId);
            
            if(deviceRegistrationsResult.IsFailed)
            {
                _logger.LogWarning("Could not get device tokens for the account {AccountId}", e.Activity.AccountId);
                return;
            }
            

            var (title, body) = _localizationService.GetLocalizedText()

            foreach(var deviceRegistration in deviceRegistrationsResult.Value)
            {
                try 
                {
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
        
        

        // This is for enum
        private bool TryGetNotificationType(ActivityEvent activityEvent, out NotificationType? type)
        {
            if(_notificationTypeMapping.ContainsKey(activityEvent.Activity.Event))
            {
                type = null;
                return false;
            }

            type = _notificationTypeMapping[activityEvent.Activity.Event];

            return true;
        }


        // This is for separate notifiction types
       // private bool TryGetNotificationType(ActivityEvent activityEvent, out NotificationMessage? message)
       // {
       //     switch(activityEvent.Activity.Event)
       //     {
       //         case ActivityTypeContract.AccountTradingDisabled:
       //             message = AccountLockedNotification.FromActivityEvent(activityEvent);
       //             return true;

       //         case ActivityTypeContract.AccountTradingEnabled:
       //             message = AccountUnlockedNotification.FromActivityEvent(activityEvent);
       //             return true;

       //         case ActivityTypeContract.AccountDepositSucceeded:
       //             message = DepositSucceededNotification.FromActivityEvent(activityEvent);
       //             return true;

       //         case ActivityTypeContract.AccountDepositFailed:
       //             message = DepositFailedNotification.FromActivityEvent(activityEvent);
       //             return true;

       //         case ActivityTypeContract.AccountWithdrawalSucceeded:
       //             message = WithdrawalSucceededNotification.FromActivityEvent(activityEvent);
       //             return true;

       //         case ActivityTypeContract.AccountWithdrawalFailed:
       //             message = WithdrawalFailedNotification.FromActivityEvent(activityEvent);
       //             return true;

       //         case ActivityTypeContract.AccountWithdrawalEnabled:
       //             message = CashUnlockedNotification.FromActivityEvent(activityEvent);
       //             return true;

       //         case ActivityTypeContract.AccountWithdrawalDisabled:
       //             message = CashLockedNotification.FromActivityEvent(activityEvent);
       //             return true;

       //         case ActivityTypeContract.MarginCall1:
       //             message = MarginCall1Notification.FromActivityEvent(activityEvent);
       //             return true;

       //         case ActivityTypeContract.MarginCall2:
       //             message = MarginCall2Notification.FromActivityEvent(activityEvent);
       //             return true;

       //         case ActivityTypeContract.Liquidation:
       //             message = LiquidationNotification.FromActivityEvent(activityEvent);
       //             return true;

       //         case ActivityTypeContract.OrderExecution:
       //         case ActivityTypeContract.OrderAcceptanceAndExecution:
       //             message = OrderExecutedNotification.FromActivityEvent(activityEvent);
       //             return true;

       //         case ActivityTypeContract.OrderExpiry:
       //             message = OrderExpiredNotification.FromActivityEvent(activityEvent);
       //             return true;

       //         case ActivityTypeContract.PositionClosing:
       //         case ActivityTypeContract.PositionPartialClosing:
       //             message = PositionClosedNotification.FromActivityEvent(activityEvent);
       //             return true;
       //     }

       //     message = null;
       //     return false;
       // }
        
    }
}
