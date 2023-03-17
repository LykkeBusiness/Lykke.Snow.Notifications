using System.Threading.Tasks;
using Lykke.Cqrs;
using Lykke.Snow.Notifications.Domain.Services;
using Lykke.Snow.Notifications.Settings;

namespace Lykke.Snow.Notifications
{
    public class StartupManager
    {
        private readonly ICqrsEngine _cqrsEngine;

        public StartupManager(ICqrsEngine cqrsEngine)
        {
            _cqrsEngine = cqrsEngine;
        }

        internal void Start()
        {
            _cqrsEngine.StartSubscribers();
        }
    }
}
