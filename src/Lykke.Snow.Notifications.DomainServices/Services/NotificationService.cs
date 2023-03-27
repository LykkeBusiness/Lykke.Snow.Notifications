using System;
using System.Threading.Tasks;
using Lykke.Snow.FirebaseIntegration.Interfaces;
using Lykke.Snow.Notifications.Domain.Model;
using Lykke.Snow.Notifications.Domain.Services;
using FirebaseAdmin.Messaging;
using Lykke.Snow.FirebaseIntegration.Exceptions;
using Lykke.Snow.Notifications.Domain.Enums;
using System.Collections.Generic;
using System.Linq;

namespace Lykke.Snow.Notifications.DomainServices.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IFcmIntegrationService _fcmIntegrationService;
        private readonly ILocalizationService _localizationService;

        public NotificationService(IFcmIntegrationService fcmIntegrationService, ILocalizationService localizationService)
        {
            _fcmIntegrationService =
                fcmIntegrationService ?? throw new ArgumentNullException(nameof(fcmIntegrationService));
            _localizationService = localizationService;
        }

        public async Task SendNotification(NotificationMessage message, string deviceToken)
        {
            ThrowIfCannotSend(message, deviceToken);
            
            var fcmMessage = MapToFcmMessage(messageArg: message, deviceToken: deviceToken);
            
            try
            {
                await _fcmIntegrationService.SendNotification(message: fcmMessage); 
            }
            catch(ArgumentNullException)
            {
                throw;
            }
            catch(ArgumentException)
            {
                throw;
            }
            catch(CannotSendNotificationException)
            {
                throw;
            }
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

        public NotificationMessage BuildLocalizedNotificationMessage(NotificationType notificationType, string[] args, string locale)
        {
            var (title, body) = _localizationService.GetLocalizedText(
                notificationType: Enum.GetName(notificationType), 
                language: locale, 
                parameters: args);

            var notificationMessage = new NotificationMessage(
                title, 
                body, 
                notificationType, 
                new Dictionary<string, string>());
            
            return notificationMessage;
        }

        public bool IsDeviceTargeted(DeviceConfiguration deviceConfiguration, NotificationType type)
        {
            return deviceConfiguration.IsNotificationEnabled(type);
        }
    }
}
