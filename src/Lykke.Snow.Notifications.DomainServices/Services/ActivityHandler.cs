using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using FirebaseAdmin.Messaging;
using Lykke.MarginTrading.Activities.Contracts.Models;
using Lykke.Snow.FirebaseIntegration.Exceptions;
using Lykke.Snow.Notifications.Domain.Enums;
using Lykke.Snow.Notifications.Domain.Repositories;
using Lykke.Snow.Notifications.Domain.Services;
using Lykke.Snow.Notifications.DomainServices.Mapping;
using Lykke.Snow.Notifications.DomainServices.Projections;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Lykke.Snow.Notifications.DomainServices.Services
{
    public class ActivityHandler : IActivityHandler
    {
        private readonly ILogger<ActivityProjection> _logger;

        private readonly INotificationService _notificationService;

        private readonly IDeviceRegistrationService _deviceRegistrationService;

        private readonly IDeviceConfigurationRepository _deviceConfigurationRepository;

        private readonly ILocalizationService _localizationService;

        public ActivityHandler(ILogger<ActivityProjection> logger,
            INotificationService notificationService,
            IDeviceRegistrationService deviceRegistrationService,
            ILocalizationService localizationService,
            IDeviceConfigurationRepository deviceConfigurationRepository)
        {
            _logger = logger;
            _notificationService = notificationService;
            _deviceRegistrationService = deviceRegistrationService;
            _deviceConfigurationRepository = deviceConfigurationRepository;
            _localizationService = localizationService;
        }

        public async Task Handle(ActivityEvent e)
        {
            var isOnBehalf = IsOnBehalf(e);

            if(!TryGetNotificationType(ActivityTypeMapping.NotificationTypeMapping, activityType: e.Activity.Event, isOnBehalf: isOnBehalf, out var notificationType))
            {
                _logger.LogDebug("No notification type mapping found for the activity {Activity}", e.Activity.Event);
                return;
            }
            
            var deviceRegistrationsResult = await _deviceRegistrationService.GetDeviceRegistrationsAsync(accountId: e.Activity.AccountId);

            if(deviceRegistrationsResult.IsFailed)
            {
                _logger.LogWarning("Could not get device tokens for the account {AccountId}. ErrorCode: {ErrorCode}", e.Activity.AccountId, deviceRegistrationsResult.Error);
                return;
            }

            _logger.LogDebug("{NumOfRegistrations} registrations found for the account {AccountId}", deviceRegistrationsResult.Value.Count(), e.Activity.AccountId);
            
            // Not all activities have enough number of description attributes
            // to fill in localization template. Here we enrich them.
            var notificationArguments = EnrichActivityDescriptions(ActivityTypeMapping.DescriptionEnrichments, e);

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
                        
                    if(!_notificationService.IsDeviceTargeted(deviceConfiguration, notificationType))
                    {
                        _logger.LogDebug("The notification has not been sent to the device {DeviceToken} because it is not targeted for the notification type {NotificationType}",
                            deviceRegistration.DeviceToken, notificationType);
                        continue;
                    }
                        
                    var (title, body) = await _localizationService.GetLocalizedTextAsync(
                        Enum.GetName(notificationType), 
                        Enum.GetName(deviceConfiguration.Locale), 
                        notificationArguments);
                
                    var notificationMessage = _notificationService.BuildNotificationMessage(
                        notificationType,
                        title,
                        body,
                        new Dictionary<string, string>()
                    );
                    
                    _logger.LogDebug("Attempting to send the notification to the account {AccountId} device {DeviceToken}", deviceRegistration.AccountId, deviceRegistration.DeviceToken);

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

        public static bool TryGetNotificationType(IReadOnlyDictionary<Tuple<ActivityTypeContract, OnBehalf>, NotificationType> notificationTypeMapping, 
            ActivityTypeContract activityType, 
            bool isOnBehalf,
            out NotificationType type)
        {
            var key = new Tuple<ActivityTypeContract, OnBehalf>(activityType, isOnBehalf ? OnBehalf.Yes : OnBehalf.No);
            
            if(!notificationTypeMapping.ContainsKey(key))
            {
                type = NotificationType.NotSpecified;
                return false;
            }
            
            type = notificationTypeMapping[key];

            return true;
        }

        public static string[] EnrichActivityDescriptions(IReadOnlyDictionary<ActivityTypeContract, Func<ActivityEvent, string[]>> enrichments, ActivityEvent e)
        {
            if(enrichments.ContainsKey(e.Activity.Event))
               return enrichments[e.Activity.Event](e);
            
            return e.Activity.DescriptionAttributes;
        }
        
        public static bool IsOnBehalf(ActivityEvent e)
        {
            try
            {
                dynamic? additionalInfo = JsonConvert.DeserializeObject(e.Activity.AdditionalInfo);
                
                if(additionalInfo == null)
                    return false;

                if (additionalInfo["IsOnBehalf"] == null)
                    return false;

                return additionalInfo["IsOnBehalf"];
            }
            catch(Exception)
            {
                return false;
            }
        }
    }
}
