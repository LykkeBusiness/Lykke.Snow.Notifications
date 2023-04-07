using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using FirebaseAdmin.Messaging;
using Lykke.Snow.FirebaseIntegration.Exceptions;
using Lykke.Snow.Notifications.Domain.Enums;
using Lykke.Snow.Notifications.Domain.Model;
using Lykke.Snow.Notifications.Domain.Repositories;
using Lykke.Snow.Notifications.Domain.Services;
using Lykke.Snow.Notifications.DomainServices.Mapping;
using Meteor.Client.Models;
using Microsoft.Extensions.Logging;

namespace Lykke.Snow.Notifications.DomainServices.Services
{
    public class MessagePreviewEventHandler : IMessagePreviewEventHandler
    {
        private readonly IDeviceRegistrationService _deviceRegistrationService;
        private readonly INotificationService _notificationService;
        private readonly IDeviceConfigurationRepository _deviceConfigurationRepository;
        private readonly ILogger<MessagePreviewEventHandler> _logger;
        private readonly ILocalizationService _localizationService;

        public MessagePreviewEventHandler(ILogger<MessagePreviewEventHandler> logger,
            IDeviceRegistrationService deviceRegistrationService,
            INotificationService notificationService,
            IDeviceConfigurationRepository deviceConfigurationRepository,
            ILocalizationService localizationService)
        {
            _logger = logger;
            _deviceRegistrationService = deviceRegistrationService;
            _notificationService = notificationService;
            _deviceConfigurationRepository = deviceConfigurationRepository;
            _localizationService = localizationService;
        }

        public async Task Handle(MessagePreviewEvent e)
        {
            _logger.LogInformation("A new MessagePreviewEvent has arrived {Event}", e.ToJson());
            
            if(e == null || e.Recipients == null)
                return;

            if(!TryGetNotificationType(MeteorMessageMapping.NotificationTypeMapping, e.Event, out var notificationType))
            {
                _logger.LogWarning("Could not find a notification type for the event type {EventType}", e.Event);
                return;
            }

            var accountIds = e.Recipients.ToArray();

            var deviceRegistrationsResult = await _deviceRegistrationService.GetDeviceRegistrationsAsync(accountIds: accountIds);

            if (deviceRegistrationsResult.IsFailed)
            {
                _logger.LogWarning("Could not get device tokens for the list of Account ids {AccountIds}. ErrorCode: {ErrorCode}",
                    string.Join(", ", accountIds), deviceRegistrationsResult.Error);

                return;
            }
               
            foreach(var deviceRegistration in deviceRegistrationsResult.Value)
            {
                try 
                {
                    var deviceConfiguration = await _deviceConfigurationRepository.GetAsync(deviceId: deviceRegistration.DeviceId, accountId: deviceRegistration.AccountId);

                    if(deviceConfiguration == null)
                    {
                        _logger.LogWarning("Device configuration could not be found for the device {DeviceId} and account {AccountId}", 
                            deviceRegistration.DeviceId, deviceRegistration.AccountId);
                        return;
                    }
                       
                    if(!_notificationService.IsDeviceTargeted(deviceConfiguration, NotificationType.InboxMessage))
                        continue;
                    
                    string title, body = "";
                    
                    if(notificationType == NotificationType.InboxMessage)
                    {
                        title = e.Subject ?? throw new ArgumentNullException();
                        body = e.Content ?? throw new ArgumentNullException();
                    }
                    
                    else
                    {
                        (title, body) = await _localizationService.GetLocalizedTextAsync(
                            Enum.GetName(notificationType), 
                            Enum.GetName(deviceConfiguration.Locale), 
                            e.LocalizationAttributes ?? new string[] {});
                    }

                    var notificationMessage = _notificationService.BuildNotificationMessage(
                        notificationType,
                        title,
                        body,
                        new Dictionary<string, string>()
                    );

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

        public static bool TryGetNotificationType(IReadOnlyDictionary<MessageEventType, NotificationType> notificationTypeMapping, MessageEventType messageType, out NotificationType type)
        {
            if(!notificationTypeMapping.ContainsKey(messageType))
            {
                type = NotificationType.NotSpecified;
                return false;
            }
            
            type = notificationTypeMapping[messageType];

            return true;
        }
    }
}
