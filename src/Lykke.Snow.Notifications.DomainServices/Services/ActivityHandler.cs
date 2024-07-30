using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using FirebaseAdmin.Messaging;
using Lykke.MarginTrading.Activities.Contracts.Helpers;
using Lykke.MarginTrading.Activities.Contracts.Models;
using Lykke.Snow.Domain.Math;
using Lykke.Snow.FirebaseIntegration.Exceptions;
using Lykke.Snow.Notifications.Domain.Enums;
using Lykke.Snow.Notifications.Domain.Repositories;
using Lykke.Snow.Notifications.Domain.Services;
using Lykke.Snow.Notifications.DomainServices.Mapping;
using Lykke.Snow.Notifications.DomainServices.Projections;
using Microsoft.Extensions.Logging;

namespace Lykke.Snow.Notifications.DomainServices.Services
{
    public class ActivityHandler : IActivityHandler
    {
        private readonly ILogger<ActivityProjection> _logger;

        private readonly INotificationService _notificationService;

        private readonly IDeviceRegistrationService _deviceRegistrationService;

        private readonly IDeviceConfigurationRepository _deviceConfigurationRepository;

        private readonly ILocalizationService _localizationService;

        private readonly IAssetService _assetService;

        public ActivityHandler(ILogger<ActivityProjection> logger,
            INotificationService notificationService,
            IDeviceRegistrationService deviceRegistrationService,
            ILocalizationService localizationService,
            IDeviceConfigurationRepository deviceConfigurationRepository,
            IAssetService assetService)
        {
            _logger = logger;
            _notificationService = notificationService;
            _deviceRegistrationService = deviceRegistrationService;
            _deviceConfigurationRepository = deviceConfigurationRepository;
            _localizationService = localizationService;
            _assetService = assetService;
        }

        public async Task Handle(ActivityEvent e)
        {
            e.Activity = await ConvertQuantitiesToUserFriendlyFormatIfNeeded(e.Activity);
            
            if(!TryGetNotificationType(ActivityTypeMapping.NotificationTypeMapping, activityType: e.Activity.Event, isOnBehalf: e.Activity.IsOnBehalf, out var notificationType))
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

                    var result = await _notificationService.SendNotification(notificationMessage, deviceToken: deviceRegistration.DeviceToken);

                    if (result)
                    {
                        _logger.LogInformation("Push notification has successfully been sent to the Account {AccountId} device {DeviceToken}: {PushNotificationPayload}",
                            deviceRegistration.AccountId, deviceRegistration.DeviceToken, notificationMessage.ToJson());
                    }
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
        
        public async Task<ActivityContract> ConvertQuantitiesToUserFriendlyFormatIfNeeded(ActivityContract activity)
        {
            var indexesToFix = DescriptionAttributesHelper.GetQtyIndexes(activity);

            if (!indexesToFix.Any())
            {
                return activity;
            }
            
            var assetId = activity.Instrument;
            var contractSize = await _assetService.GetContractSize(assetId);
            if (!contractSize.HasValue)
            {
                _logger.LogWarning($"Couldn't find asset with id {assetId} in cache for activity {activity.ToJson()}");
                return activity;
            }
            
            if (contractSize < 1)
            {
                _logger.LogWarning($"Invalid contract size for asset {assetId}.");
                return activity;
            }

            if (contractSize == 1)
            {
                return activity;
            }

            foreach (int index in indexesToFix)
            {
                if (index >= activity.DescriptionAttributes.Length)
                {
                    _logger.LogWarning($"Couldn't find index {index} in description attributes for activity {activity.ToJson()}");
                    continue;
                }
                
                if (!int.TryParse(activity.DescriptionAttributes[index], out int qty))
                {
                    _logger.LogWarning($"Couldn't parse quantity with {index} in description attributes for activity {activity.ToJson()}");
                    continue;
                }
                
                var userFriendlyQty = QuantityMath.Compute(qty, contractSize);
                activity.DescriptionAttributes[index] = userFriendlyQty.ToString();
            }

            return activity;
        }

        public static bool TryGetNotificationType(IReadOnlyDictionary<Tuple<ActivityTypeContract, OnBehalf>, NotificationType> notificationTypeMapping, 
            ActivityTypeContract activityType, 
            bool? isOnBehalf,
            out NotificationType type)
        {
            var key = new Tuple<ActivityTypeContract, OnBehalf>(activityType, (isOnBehalf.HasValue && isOnBehalf.Value) ? OnBehalf.Yes : OnBehalf.No);
            
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
    }
}
