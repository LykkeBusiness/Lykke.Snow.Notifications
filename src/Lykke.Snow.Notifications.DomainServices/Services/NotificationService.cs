using System;
using System.Threading.Tasks;
using Lykke.Snow.Notifications.Domain.Exceptions;
using Lykke.Snow.Notifications.Domain.Model;
using Lykke.Snow.Notifications.Domain.Services;
using Microsoft.Extensions.Logging;

namespace Lykke.Snow.Notifications.DomainServices.Services
{
    public class NotificationService : INotificationService
    {
        private bool _isInitialized = false;
        public bool IsInitialized => _isInitialized;
        private readonly IFcmService _fcmService;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(IFcmService fcmService, ILogger<NotificationService> logger)
        {
            _fcmService = fcmService ?? throw new ArgumentNullException(nameof(fcmService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Initialize()
        {
            if(_isInitialized)
                return;
            
            try 
            {
                _fcmService.CreateApp();
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
            
            return _fcmService.SendNotificationToDevice(message, deviceToken); 
        }
        
        private void ThrowIfCannotSend(string deviceToken)
        {
            if(!_isInitialized)
                throw new NotificationServiceNotInitializedException();
            
            if(string.IsNullOrEmpty(deviceToken))
                throw new ArgumentNullException(nameof(deviceToken));
        }
    }
}
