using System;
using System.Threading.Tasks;
using Common;
using Lykke.RabbitMqBroker;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.RabbitMqBroker.Subscriber.Deserializers;
using Lykke.RabbitMqBroker.Subscriber.MessageReadStrategies;
using Lykke.Snow.Notifications.Settings;
using Lykke.Snow.Notifications.Subscribers.Messages;
using Microsoft.Extensions.Logging;

namespace Lykke.Snow.Notifications.Subscribers
{
    public class MessagePreviewSubscriber : IStartStop
    {
        private RabbitMqPullingSubscriber<MessagePreviewEvent> _subscriber;
        private readonly ILoggerFactory _loggerFactory;
        private readonly RabbitMqSubscriptionSettings _settings;
        private readonly ILogger<MessagePreviewSubscriber> _logger;

        public MessagePreviewSubscriber(ILoggerFactory loggerFactory, 
            SubscriptionSettings settings, 
            ILogger<MessagePreviewSubscriber> logger)
        {
            _loggerFactory = loggerFactory;
            _settings = settings;
            _logger = logger;
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

        private Task ProcessMessageAsync(MessagePreviewEvent arg)
        {
            //TODO: process message
            _logger.LogInformation("A new MessagePreviewEvent has arrived {Event}", arg.ToJson());
            
            return Task.CompletedTask;
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
