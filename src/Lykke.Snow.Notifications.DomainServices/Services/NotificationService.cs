using System;
using System.Threading.Tasks;
using Lykke.Snow.FirebaseIntegration.Interfaces;
using Lykke.Snow.Notifications.Domain.Model;
using Lykke.Snow.Notifications.Domain.Services;
using FirebaseAdmin.Messaging;
using Lykke.Snow.FirebaseIntegration.Exceptions;
using Microsoft.Extensions.Logging;

namespace Lykke.Snow.Notifications.DomainServices.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IFcmIntegrationService _fcmIntegrationService;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(IFcmIntegrationService fcmIntegrationService, ILogger<NotificationService> logger)
        {
            _fcmIntegrationService =
                fcmIntegrationService ?? throw new ArgumentNullException(nameof(fcmIntegrationService));
            _logger = logger;
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
            catch(CannotSendNotificationException e)
            {
                // TODO: Now is it a good time to remove it from the db?
                if(e.ErrorCode == MessagingErrorCode.Unregistered)
                {
                    _logger.LogWarning("Notification could not be delivered to the device {DeviceToken} becuase the device is no longer active.");
                    return;
                }

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
    }
}
