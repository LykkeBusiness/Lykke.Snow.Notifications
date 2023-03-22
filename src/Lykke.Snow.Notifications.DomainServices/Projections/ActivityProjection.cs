using System;
using System.Threading.Tasks;
using Common;
using JetBrains.Annotations;
using Lykke.MarginTrading.Activities.Contracts.Models;
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

            var notificationMessage = GetNotificationType(e);
            
            var deviceRegistrationsResult = await _deviceRegistrationService.GetDeviceRegistrationsAsync(accountId: e.Activity.AccountId);
            
            if(deviceRegistrationsResult.IsFailed)
            {
                _logger.LogWarning("Could not get device tokens for the account {AccountId}", e.Activity.AccountId);
                return;
            }

            foreach(var deviceRegistration in deviceRegistrationsResult.Value)
            {
                await _notificationService.SendNotification(notificationMessage, deviceToken: deviceRegistration.DeviceToken);

                _logger.LogInformation("Push notification has successfully been sent to the device {DeviceToken}: {PushNotificationPayload}",
                    deviceRegistration.DeviceToken, notificationMessage.ToJson());
            }
        }
        
        private NotificationMessage GetNotificationType(ActivityEvent activityEvent)
        {
            switch(activityEvent.Activity.Event)
            {
                case ActivityTypeContract.AccountTradingDisabled:
                case ActivityTypeContract.AccountTradingEnabled:
                    return AccountLockNotification.FromActivityEvent(activityEvent);

                case ActivityTypeContract.AccountDepositSucceeded:
                case ActivityTypeContract.AccountDepositFailed:
                    return DepositNotification.FromActivityEvent(activityEvent);

                case ActivityTypeContract.AccountWithdrawalSucceeded:
                case ActivityTypeContract.AccountWithdrawalFailed:
                    return WithdrawalNotification.FromActivityEvent(activityEvent);

                case ActivityTypeContract.AccountWithdrawalEnabled:
                case ActivityTypeContract.AccountWithdrawalDisabled:
                    return CashLockNotification.FromActivityEvent(activityEvent);

                case ActivityTypeContract.MarginCall1:
                    return MarginCall1Notification.FromActivityEvent(activityEvent);

                case ActivityTypeContract.MarginCall2:
                    return MarginCall2Notification.FromActivityEvent(activityEvent);

                case ActivityTypeContract.Liquidation:
                    return LiquidationNotification.FromActivityEvent(activityEvent);

                case ActivityTypeContract.OrderExecution:
                    return OrderExecutedNotification.FromActivityEvent(activityEvent);

                case ActivityTypeContract.OrderExpiry:
                    return OrderExpiredNotification.FromActivityEvent(activityEvent);

                case ActivityTypeContract.PositionClosing:
                    return PositionClosedNotification.FromActivityEvent(activityEvent);
                
                default:
                    throw new ArgumentException(); //TODO: Create a custom exception
            }
        }
        
    }
}
