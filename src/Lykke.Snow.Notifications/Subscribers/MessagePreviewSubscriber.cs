using System;
using System.Linq;
using System.Threading.Tasks;
using Common;
using FirebaseAdmin.Messaging;
using Lykke.RabbitMqBroker;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.RabbitMqBroker.Subscriber.Deserializers;
using Lykke.RabbitMqBroker.Subscriber.MessageReadStrategies;
using Lykke.Snow.FirebaseIntegration.Exceptions;
using Lykke.Snow.Notifications.Domain.Enums;
using Lykke.Snow.Notifications.Domain.Repositories;
using Lykke.Snow.Notifications.Domain.Services;
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
        private readonly IDeviceRegistrationService _deviceRegistrationService;
        private readonly IDeviceConfigurationRepository _deviceConfigurationRepository;
        private readonly INotificationService _notificationService;

        public MessagePreviewSubscriber(ILoggerFactory loggerFactory,
            SubscriptionSettings settings,
            ILogger<MessagePreviewSubscriber> logger,
            IDeviceRegistrationService deviceRegistrationService,
            IDeviceConfigurationRepository deviceConfigurationRepository,
            INotificationService notificationService)
        {
            _loggerFactory = loggerFactory;
            _settings = settings;
            _logger = logger;
            _deviceRegistrationService = deviceRegistrationService;
            _deviceConfigurationRepository = deviceConfigurationRepository;
            _notificationService = notificationService;
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

        private async Task ProcessMessageAsync(MessagePreviewEvent arg)
        {
            _logger.LogInformation("A new MessagePreviewEvent has arrived {Event}", arg.ToJson());
            
            // TODO: may need to do nullity check against arg.Recipients.
            
            var accountIds = arg.Recipients.First();
            
           var deviceRegistrationsResult = await _deviceRegistrationService.GetDeviceRegistrationsAsync(accountId: accountId);
           
           if(deviceRegistrationsResult.IsFailed)
           {
               _logger.LogWarning("Could not get device tokens for the account {AccountId}. ErrorCode: {ErrorCode}", accountId, deviceRegistrationsResult.Error);
               return;
           }

           foreach(var deviceRegistration in deviceRegistrationsResult.Value)
           {
               try 
               {
                   var deviceConfiguration = await _deviceConfigurationRepository.GetAsync(deviceId: deviceRegistration.DeviceId);
                       
                   if(!_notificationService.IsDeviceTargeted(deviceConfiguration, NotificationType.InboxMessage))
                       continue;
               
                   var notificationMessage = _notificationService.BuildNotificationMessage(NotificationType.InboxMessage, 
                        title: "Inbox",
                        body: arg.Content);

                   await _notificationService.SendNotification(notificationMessage, deviceToken: deviceRegistration.DeviceToken);

                   _logger.LogInformation("Push notification has successfully been sent to the device {DeviceToken}: {PushNotificationPayload}",
                       deviceRegistration.DeviceToken, notificationMessage.ToJson());
               }
               catch(CannotSendNotificationException ex)
               {
                   if(ex.ErrorCode == MessagingErrorCode.Unregistered)
                   {
                       _logger.LogWarning("The notification could not be delivered to the device {DeviceToken} because it is no longer active.", deviceRegistration.DeviceToken);
                   }
               }
           }
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
