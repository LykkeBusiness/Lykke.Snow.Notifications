using System;
using System.Threading.Tasks;
using Lykke.Snow.FirebaseIntegration.Interfaces;
using Lykke.Snow.Notifications.Domain.Exceptions;
using Lykke.Snow.Notifications.Domain.Model;
using Lykke.Snow.Notifications.Domain.Services;
using Microsoft.Extensions.Logging;
using FirebaseAdmin.Messaging;

namespace Lykke.Snow.Notifications.DomainServices.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IFcmIntegrationService _fcmIntegrationService;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(IFcmIntegrationService fcmIntegrationService, ILogger<NotificationService> logger)
        {
            _fcmIntegrationService = fcmIntegrationService ?? throw new ArgumentNullException(nameof(fcmIntegrationService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task SendNotification(NotificationMessage message, string deviceToken)
        {
            ThrowIfCannotSend(deviceToken);
            
            var fcmMessage = MapToFcmMessage(messageArg: message, deviceToken: deviceToken);
            
            try
            {
                await _fcmIntegrationService.SendNotification(message: fcmMessage, deviceToken: deviceToken); 
            }
            catch(Exception e)
            {
                _logger.LogError(e, "An error has occured while trying to send the notification.");
            }
        }
        
        private void ThrowIfCannotSend(string deviceToken)
        {
            if(string.IsNullOrEmpty(deviceToken))
                throw new ArgumentNullException(nameof(deviceToken));
        }
        
        public FirebaseAdmin.Messaging.Message MapToFcmMessage(NotificationMessage messageArg, string deviceToken)
        {
            return new FirebaseAdmin.Messaging.Message()
            {
                Token = deviceToken,
                Notification = new Notification()
                {
                    Title = messageArg.Title,
                    Body = messageArg.Body
                },
                Data = messageArg.KeyValueCollection
            };
        }
    }
}
