using System.Threading.Tasks;
using Lykke.Cqrs;
using Lykke.Snow.Notifications.Domain.Services;
using Lykke.Snow.Notifications.Settings;

namespace Lykke.Snow.Notifications
{
    public class StartupManager
    {
        private readonly ICqrsEngine _cqrsEngine;
        private readonly INotificationService _notificationService;
        private readonly NotificationServiceSettings _settings;

        public StartupManager(ICqrsEngine cqrsEngine, 
            INotificationService notificationService, 
            NotificationServiceSettings settings)
        {
            _cqrsEngine = cqrsEngine;
            _notificationService = notificationService;
            _settings = settings;
        }

        internal async Task Start()
        {
            _cqrsEngine.StartSubscribers();
        }
    }
}
