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
using Microsoft.Extensions.Logging;

namespace Lykke.Snow.Notifications.DomainServices.Services
{
    public class MessagePreviewEventHandler : IMessagePreviewEventHandler
    {
        private readonly IDeviceRegistrationService _deviceRegistrationService;
        private readonly INotificationService _notificationService;
        private readonly IDeviceConfigurationRepository _deviceConfigurationRepository;
        private readonly ILogger<MessagePreviewEventHandler> _logger;

        public MessagePreviewEventHandler(ILogger<MessagePreviewEventHandler> logger,
            IDeviceRegistrationService deviceRegistrationService,
            INotificationService notificationService,
            IDeviceConfigurationRepository deviceConfigurationRepository)
        {
            _logger = logger;
            _deviceRegistrationService = deviceRegistrationService;
            _notificationService = notificationService;
            _deviceConfigurationRepository = deviceConfigurationRepository;
        }

        public async Task Handle(MessagePreviewEvent e)
        {
            _logger.LogDebug("A new MessagePreviewEvent has arrived {Event}", e.ToJson());
            
            if(e == null || e.Recipients == null || string.IsNullOrEmpty(e.Subject) || string.IsNullOrEmpty(e.Content))
            {
                _logger.LogDebug("Notification is not attempted becase Message is not valid. One of these properties might be empty: Subject, Content, Recipients");
                return;
            }

            var accountIds = e.Recipients.ToArray();

            var deviceRegistrationsResult = await _deviceRegistrationService.GetDeviceRegistrationsAsync(accountIds: accountIds);

            _logger.LogDebug("{NumOfRegistrations} registrations found for the accounts {AccountIds}", deviceRegistrationsResult.Value.Count(), string.Join(',', accountIds));

            if (deviceRegistrationsResult.IsFailed)
            {
                _logger.LogWarning("Could not get device tokens for the list of Account ids {AccountIds}. ErrorCode: {ErrorCode}",
                    string.Join(", ", accountIds), deviceRegistrationsResult.Error);

                return;
            }
               
            var notificationMessage = _notificationService.BuildNotificationMessage(NotificationType.InboxMessage, 
                title: e.Subject,
                body: e.Content,
                keyValuePairs: new Dictionary<string, string>());

            foreach(var deviceRegistration in deviceRegistrationsResult.Value)
            {
                try 
                {
                    var deviceConfiguration = await _deviceConfigurationRepository.GetAsync(deviceId: deviceRegistration.DeviceId, accountId: deviceRegistration.AccountId);

                    if(deviceConfiguration == null)
                    {
                        _logger.LogWarning("Device configuration could not be found for the device {DeviceId} and account {AccountId}", 
                            deviceRegistration.DeviceId, deviceRegistration.AccountId);
                        continue;
                    }
                       
                    if(!_notificationService.IsDeviceTargeted(deviceConfiguration, NotificationType.InboxMessage))
                    {
                        _logger.LogDebug("The notification has not been sent to the device {DeviceToken} because it is not targeted for Inbox Messages",
                            deviceRegistration.DeviceToken);
                        continue;
                    }

                    _logger.LogDebug("Attempting to send the notification to the device {DeviceToken}", deviceRegistration.DeviceToken);

                    await _notificationService.SendNotification(notificationMessage, deviceToken: deviceRegistration.DeviceToken);

                    _logger.LogInformation("Push notification has successfully been sent to the Account {AccountId} device {DeviceToken}: {PushNotificationPayload}",
                        deviceRegistration.AccountId, deviceRegistration.DeviceToken, notificationMessage.ToJson());
                }
                catch(CannotSendNotificationException ex)
                {
                    if(ex.ErrorCode == MessagingErrorCode.Unregistered)
                    {
                        _logger.LogDebug("The notification could not be delivered to the device {DeviceToken} because it is no longer active.", deviceRegistration.DeviceToken);
                        continue;
                    }

                    _logger.LogError(ex, "The notification could not be delivered to the device {DeviceToken}. ErrorCode: {ErrorCode}", deviceRegistration.DeviceToken, ex.ErrorCode);
                }
            }
        }
    }
}
