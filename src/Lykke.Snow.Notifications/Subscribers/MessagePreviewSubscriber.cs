using System.Threading.Tasks;
using Lykke.RabbitMqBroker;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.RabbitMqBroker.Subscriber.Deserializers;
using Lykke.RabbitMqBroker.Subscriber.MessageReadStrategies;
using Lykke.Snow.Notifications.Domain.Model;
using Lykke.Snow.Notifications.Domain.Services;
using Lykke.Snow.Notifications.Settings;
using Microsoft.Extensions.Logging;

namespace Lykke.Snow.Notifications.Subscribers
{
    public class MessagePreviewSubscriber : IStartStop
    {
        private RabbitMqPullingSubscriber<MessagePreviewEvent>? _subscriber;
        private readonly ILoggerFactory _loggerFactory;
        private readonly RabbitMqSubscriptionSettings _settings;
        private readonly ILogger<MessagePreviewSubscriber> _logger;
        private readonly IMessagePreviewEventHandler _messagePreviewEventHandler;

        public MessagePreviewSubscriber(ILoggerFactory loggerFactory,
            SubscriptionSettings settings,
            ILogger<MessagePreviewSubscriber> logger,
            IMessagePreviewEventHandler messagePreviewEventHandler)
        {
            _loggerFactory = loggerFactory;
            _settings = settings;
            _logger = logger;
            _messagePreviewEventHandler = messagePreviewEventHandler;
        }

        public void Dispose()
        {
            Stop();
        }

        public void Start()
        {
            _subscriber = new RabbitMqPullingSubscriber<MessagePreviewEvent>(
                    _loggerFactory.CreateLogger<RabbitMqPullingSubscriber<MessagePreviewEvent>>(),
                    _settings)
                    .SetMessageDeserializer(new MessagePackMessageDeserializer<MessagePreviewEvent>())
                    .SetMessageReadStrategy(new MessageReadQueueStrategy())
                    .Subscribe(ProcessMessageAsync)
                    .Start();
        }

        public async Task ProcessMessageAsync(MessagePreviewEvent e)
        {
            await _messagePreviewEventHandler.Handle(e);
        }

        public void Stop()
        {
            if(_subscriber != null)
            {
                _subscriber.Stop();
                _subscriber.Dispose();
            }
        }
    }
}
