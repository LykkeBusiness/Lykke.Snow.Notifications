using System.Threading.Tasks;
using Common;
using FirebaseAdmin.Messaging;
using JetBrains.Annotations;
using Lykke.MarginTrading.Activities.Contracts.Models;
using Lykke.Snow.FirebaseIntegration.Exceptions;
using Lykke.Snow.Notifications.Domain.Model;
using Lykke.Snow.Notifications.Domain.Model.NotificationTypes;
using Lykke.Snow.Notifications.Domain.NotificationTypes;
using Lykke.Snow.Notifications.Domain.Services;
using Microsoft.Extensions.Logging;

namespace Lykke.Snow.Notifications.DomainServices.Projections
{
    public class ActivityProjection
    {
        private readonly ILogger<ActivityProjection> _logger;
        private readonly INotificationService _notificationService;
        private readonly IDeviceRegistrationService _deviceRegistrationService;

        public ActivityProjection(ILogger<ActivityProjection> logger, 
            INotificationService notificationService, 
            IDeviceRegistrationService deviceRegistrationService)
        {
            _logger = logger;
            _notificationService = notificationService;
            _deviceRegistrationService = deviceRegistrationService;
        }

        [UsedImplicitly]
        public async Task Handle(ActivityEvent e)
        {
            _logger.LogInformation("A new activity event has just arrived {ActivityEvent}", e.ToJson());

            NotificationMessage? notificationMessage;
            
            if(!TryGetNotificationType(activityEvent: e, out notificationMessage))
                return;
        
            if(notificationMessage == null)
                return;

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
        
        private bool TryGetNotificationType(ActivityEvent activityEvent, out NotificationMessage? message)
        {
            switch(activityEvent.Activity.Event)
            {
                case ActivityTypeContract.AccountTradingDisabled:
                    message = AccountLockNotification.FromActivityEvent(activityEvent, locked: true);
                    return true;

                case ActivityTypeContract.AccountTradingEnabled:
                    message = AccountLockNotification.FromActivityEvent(activityEvent, locked: false);
                    return true;

                case ActivityTypeContract.AccountDepositSucceeded:
                    message = DepositNotification.FromActivityEvent(activityEvent, success: true);
                    return true;

                case ActivityTypeContract.AccountDepositFailed:
                    message = DepositNotification.FromActivityEvent(activityEvent, success: false);
                    return true;

                case ActivityTypeContract.AccountWithdrawalSucceeded:
                    message = WithdrawalNotification.FromActivityEvent(activityEvent, success: true);
                    return true;

                case ActivityTypeContract.AccountWithdrawalFailed:
                    message = WithdrawalNotification.FromActivityEvent(activityEvent, success: false);
                    return true;

                case ActivityTypeContract.AccountWithdrawalEnabled:
                case ActivityTypeContract.AccountWithdrawalDisabled:
                    message = CashLockNotification.FromActivityEvent(activityEvent);
                    return true;

                case ActivityTypeContract.MarginCall1:
                    message = MarginCall1Notification.FromActivityEvent(activityEvent);
                    return true;

                case ActivityTypeContract.MarginCall2:
                    message = MarginCall2Notification.FromActivityEvent(activityEvent);
                    return true;

                case ActivityTypeContract.Liquidation:
                    message = LiquidationNotification.FromActivityEvent(activityEvent);
                    return true;

                case ActivityTypeContract.OrderExecution:
                    message = OrderExecutedNotification.FromActivityEvent(activityEvent);
                    return true;

                case ActivityTypeContract.OrderExpiry:
                    message = OrderExpiredNotification.FromActivityEvent(activityEvent);
                    return true;

                case ActivityTypeContract.PositionClosing:
                    message = PositionClosedNotification.FromActivityEvent(activityEvent);
                    return true;
            }

            message = null;
            return false;
        }
        
    }
}
