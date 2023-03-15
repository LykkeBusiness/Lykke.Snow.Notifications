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
        private bool _isInitialized = false;
        public bool IsInitialized => _isInitialized;
        private readonly IFcmIntegrationService _fcmIntegrationService;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(IFcmIntegrationService fcmIntegrationService, ILogger<NotificationService> logger)
        {
            _fcmIntegrationService = fcmIntegrationService ?? throw new ArgumentNullException(nameof(fcmIntegrationService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Initialize()
        {
            if(_isInitialized)
                return;
            
            try 
            {
                _fcmIntegrationService.CreateApp();
                _isInitialized = true;
            }
            catch(Exception e)
            {
                _logger.LogError(e, $"Could not initialize {nameof(NotificationService)}!");
                throw;
            }
        }

        public Task SendNotificationToSingleDevice(NotificationMessage message, string deviceToken)
        {
            ThrowIfCannotSend(deviceToken);
            
            var fcmMessage = MapToFcmMessage(messageArg: message, deviceToken: deviceToken);
            
            return _fcmIntegrationService.SendNotificationToDevice(message: fcmMessage, deviceToken: deviceToken); 
        }
        
        private void ThrowIfCannotSend(string deviceToken)
        {
            if(!_isInitialized)
                throw new NotificationServiceNotInitializedException();
            
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
                Data = messageArg.KeyValueBag
            };
        }
    }
}
