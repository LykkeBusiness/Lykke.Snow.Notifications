using System;
using System.Threading.Tasks;
using Lykke.Snow.FirebaseIntegration.Interfaces;
using Lykke.Snow.Notifications.Domain.Model;
using Lykke.Snow.Notifications.Domain.Services;
using FirebaseAdmin.Messaging;
using Lykke.Snow.Notifications.Domain.Enums;
using System.Collections.Generic;
using Lykke.Snow.Notifications.Domain.Extensions;
using Microsoft.Extensions.Logging;

namespace Lykke.Snow.Notifications.DomainServices.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IFcmIntegrationService _fcmIntegrationService;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(IFcmIntegrationService fcmIntegrationService, ILogger<NotificationService> logger)
        {
            _fcmIntegrationService = fcmIntegrationService ?? throw new ArgumentNullException(nameof(fcmIntegrationService));
            _logger = logger;
        }

        public async Task<bool> SendNotification(NotificationMessage message, string deviceToken)
        {
            ThrowIfCannotSend(message, deviceToken);
            
            var fcmMessage = MapToFcmMessage(messageArg: message, deviceToken: deviceToken);

           var result = await _fcmIntegrationService.SendNotification(message: fcmMessage);
           if (result.IsFailed)
           {
               _logger.LogWarning("Failed to send message to device {deviceToken} because {reason} - {code}", deviceToken, result.Value, result.Error);
           }

           return result.IsSuccess;
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
                Data = messageArg.KeyValueCollection,
                Android = new AndroidConfig
                {
                    Priority = Priority.High,
                    Notification = new AndroidNotification
                    {
                        DefaultSound = true
                    }
                },
                Apns = new ApnsConfig
                {
                    Aps = new Aps
                    {
                        ContentAvailable = true,
                        Sound = "default"
                    }
                }
            };
        }

        public bool IsDeviceTargeted(DeviceConfiguration deviceConfiguration, NotificationType type)
        {
            if(type.IsOnBehalfNotification())
            {
                return deviceConfiguration.EnabledNotificationTypes.Contains(NotificationType.OnBehalfAction);
            }

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
