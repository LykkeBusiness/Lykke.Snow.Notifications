using System;
using System.Threading.Tasks;
using Lykke.Snow.FirebaseIntegration.Interfaces;
using Lykke.Snow.Notifications.Domain.Model;
using Lykke.Snow.Notifications.Domain.Services;
using FirebaseAdmin.Messaging;
using Lykke.Snow.Notifications.Domain.Enums;
using System.Collections.Generic;

namespace Lykke.Snow.Notifications.DomainServices.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IFcmIntegrationService _fcmIntegrationService;

        public NotificationService(IFcmIntegrationService fcmIntegrationService)
        {
            _fcmIntegrationService = fcmIntegrationService ?? throw new ArgumentNullException(nameof(fcmIntegrationService));
        }

        public async Task SendNotification(NotificationMessage message, string deviceToken)
        {
            ThrowIfCannotSend(message, deviceToken);
            
            var fcmMessage = MapToFcmMessage(messageArg: message, deviceToken: deviceToken);

            await _fcmIntegrationService.SendNotification(message: fcmMessage);
        }
        
        private void ThrowIfCannotSend(NotificationMessage message, string deviceToken)
        {
            if(string.IsNullOrEmpty(deviceToken))
                throw new ArgumentNullException(nameof(deviceToken));
                
            if(message == null)
                throw new ArgumentNullException(nameof(message));
        }
        
        public Message MapToFcmMessage(NotificationMessage messageArg, string deviceToken)
        {
            return new Message
            {
                Token = deviceToken,
                Notification = new Notification
                {
                    Title = messageArg.Title,
                    Body = messageArg.Body
                },
                Data = messageArg.KeyValueCollection
            };
        }

        public bool IsDeviceTargeted(DeviceConfiguration deviceConfiguration, NotificationType type)
        {
            return deviceConfiguration.IsNotificationEnabled(type);
        }

        public NotificationMessage BuildNotificationMessage(NotificationType notificationType, string? title, string? body, Dictionary<string, string> keyValuePairs)
        {
            if(string.IsNullOrEmpty(title))
                throw new ArgumentNullException(nameof(title));
            
            if(string.IsNullOrEmpty(body))
                throw new ArgumentNullException(nameof(body));

            var notificationMessage = new NotificationMessage(
                title, 
                body, 
                notificationType, 
                keyValuePairs);
            
            return notificationMessage;
        }
    }
}
