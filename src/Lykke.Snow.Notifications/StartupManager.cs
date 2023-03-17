using Lykke.Cqrs;

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
