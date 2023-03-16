using System;
using System.Threading.Tasks;
using Lykke.Snow.FirebaseIntegration.Interfaces;
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
            ThrowIfCannotSend(message, deviceToken);
            
            var fcmMessage = MapToFcmMessage(messageArg: message, deviceToken: deviceToken);
            
            var result = await _fcmIntegrationService.SendNotification(message: fcmMessage, deviceToken: deviceToken); 
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
            return new Message()
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
