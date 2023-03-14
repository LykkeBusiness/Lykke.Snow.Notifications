using System.Threading.Tasks;
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

        internal async Task Start()
        {
            _cqrsEngine.StartSubscribers();
        }
    }
}
