using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using FirebaseAdmin.Messaging;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.Snow.FirebaseIntegration.Exceptions;
using Lykke.Snow.Notifications.Domain.Enums;
using Lykke.Snow.Notifications.Domain.Model;
using Lykke.Snow.Notifications.Domain.Repositories;
using Lykke.Snow.Notifications.Domain.Services;
using Lykke.Snow.Notifications.DomainServices.Mapping;
using Meteor.Client.Models;
using Microsoft.Extensions.Logging;

namespace Lykke.Snow.Notifications.DomainServices.Services;

public class MessagePreviewHandler : IMessageHandler<MessagePreviewEvent>
{
    private readonly IDeviceRegistrationService _deviceRegistrationService;
    private readonly INotificationService _notificationService;
    private readonly IDeviceConfigurationRepository _deviceConfigurationRepository;
    private readonly ILogger<MessagePreviewHandler> _logger;
    private readonly ILocalizationService _localizationService;
    public MessagePreviewHandler(ILogger<MessagePreviewHandler> logger,
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
        _logger.LogDebug("A new MessagePreviewEvent has arrived {Event}", e.ToJson());

        if (e == null || e.Recipients == null)
        {
            _logger.LogDebug("Notification is not attempted becase Message is not valid. One of these properties might be empty: Subject, Content, Recipients");
            return;
        }

        if (!TryGetNotificationType(MeteorMessageMapping.NotificationTypeMapping, e.Event, out var notificationType))
        {
            _logger.LogDebug("Could not find a notification type for the event type {EventType}", e.Event);
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

        _logger.LogDebug("{NumOfRegistrations} registrations found for the accounts {AccountIds}", deviceRegistrationsResult.Value.Count(), string.Join(',', accountIds));

        foreach (var deviceRegistration in deviceRegistrationsResult.Value)
        {
            try
            {
                var deviceConfiguration = await _deviceConfigurationRepository.GetAsync(deviceId: deviceRegistration.DeviceId, accountId: deviceRegistration.AccountId);

                if (deviceConfiguration == null)
                {
                    _logger.LogWarning("Device configuration could not be found for the device {DeviceId} and account {AccountId}",
                        deviceRegistration.DeviceId, deviceRegistration.AccountId);
                    continue;
                }

                if (!_notificationService.IsDeviceTargeted(deviceConfiguration, notificationType))
                {
                    _logger.LogDebug("The notification has not been sent to the device {DeviceToken} because it is not targeted for notification type {NotificationType}",
                        deviceRegistration.DeviceToken, notificationType);
                    continue;
                }

                var notificationMessage = await BuildNotificationMessage(e, notificationType, deviceConfiguration.Locale);

                _logger.LogDebug("Attempting to send the notification to the Account {AccountId} device {DeviceToken}", deviceRegistration.AccountId, deviceRegistration.DeviceToken);

                var result = await _notificationService.SendNotification(notificationMessage, deviceToken: deviceRegistration.DeviceToken);

                if (result)
                {
                    _logger.LogInformation("Push notification has successfully been sent to the Account {AccountId} device {DeviceToken}: {PushNotificationPayload}",
                        deviceRegistration.AccountId, deviceRegistration.DeviceToken, notificationMessage.ToJson());
                }
            }
            catch (CannotSendNotificationException ex)
            {
                if (ex.ErrorCode == MessagingErrorCode.Unregistered)
                {
                    _logger.LogDebug("The notification could not be delivered to the device {DeviceToken} because it is no longer active.", deviceRegistration.DeviceToken);
                    continue;
                }

                _logger.LogError(ex, "The notification could not be delivered to the device {DeviceToken}. ErrorCode: {ErrorCode}", deviceRegistration.DeviceToken, ex.ErrorCode);
            }
        }
    }

    public static bool TryGetNotificationType(IReadOnlyDictionary<MessageEventType, NotificationType> notificationTypeMapping, MessageEventType messageType, out NotificationType type)
    {
        if (!notificationTypeMapping.ContainsKey(messageType))
        {
            type = NotificationType.NotSpecified;
            return false;
        }

        type = notificationTypeMapping[messageType];

        return true;
    }

    public async Task<NotificationMessage> BuildNotificationMessage(MessagePreviewEvent e, NotificationType notificationType, Locale locale)
    {
        string? title = e.Subject;
        string? body = e.Content;

        // We don't need localization if the notification is an inbox message
        if (notificationType != NotificationType.InboxMessage)
        {
            (title, body) = await _localizationService.GetLocalizedTextAsync(
                Enum.GetName(notificationType),
                Enum.GetName(locale),
                e.LocalizationAttributes ?? Array.Empty<string>());
        }

        return _notificationService.BuildNotificationMessage(
            notificationType,
            title,
            body,
            new Dictionary<string, string>()
        );
    }
}
