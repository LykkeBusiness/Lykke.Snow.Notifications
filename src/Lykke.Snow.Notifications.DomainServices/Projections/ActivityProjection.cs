using System;
using Common;
using JetBrains.Annotations;
using Lykke.MarginTrading.Activities.Contracts.Models;
using Lykke.Snow.Notifications.Domain.Model;
using Lykke.Snow.Notifications.Domain.NotificationTypes;
using Lykke.Snow.Notifications.Domain.Services;
using Microsoft.Extensions.Logging;

namespace Lykke.Snow.Notifications.DomainServices.Projections
{
    public class ActivityProjection
    {
        private readonly ILogger<ActivityProjection> _logger;
        private readonly INotificationService _notificationService;

        public ActivityProjection(ILogger<ActivityProjection> logger, INotificationService notificationService)
        {
            _logger = logger;
            _notificationService = notificationService;
        }

        [UsedImplicitly]
        public void Handle(ActivityEvent e)
        {
            var notificationMessage = GetNotificationType(e);
            
            // TODO: get all devicetokens that linked to the e.AccountId
            _notificationService.SendNotification(notificationMessage, deviceToken: "");
            
            _logger.LogInformation("A new activity event has just arrived {ActivityEvent}", e.ToJson());
        }
        
        private NotificationMessage GetNotificationType(ActivityEvent activityEvent)
        {
            switch(activityEvent.Activity.Event)
            {
                case ActivityTypeContract.AccountDepositSucceeded:
                    return DepositNotification.FromActivityEvent(activityEvent);
                default:
                    throw new ArgumentException(); //TODO: Create a custom exception
            }
        }
        
    }
}
