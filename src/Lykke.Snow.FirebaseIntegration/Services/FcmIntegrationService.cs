using System;
using System.Threading.Tasks;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Lykke.Snow.Common.Model;
using Lykke.Snow.FirebaseIntegration.Exceptions;
using Lykke.Snow.FirebaseIntegration.Interfaces;
using Microsoft.Extensions.Logging;

namespace Lykke.Snow.FirebaseIntegration.Services
{
    /// <summary>
    /// Firebase Cloud Messaging client. Communicates to Firebase services.
    /// </summary>
    public class FcmIntegrationService : IFcmIntegrationService
    {
        private readonly ILogger<FcmIntegrationService> _logger;

        public FcmIntegrationService(IFcmOptionsFactory optionsFactory, ILogger<FcmIntegrationService> logger)
        {
            Initialize(optionsFactory);
            _logger = logger;
        }

        public async Task<Result<string, MessagingErrorCode>> SendNotification(Message fcmMessage)
        {
            try 
            {
                var response = await FirebaseMessaging.DefaultInstance.SendAsync(fcmMessage);

                return new Result<string, MessagingErrorCode>(value: response);
            }
            catch(ArgumentNullException e)
            {
                throw new NotificationMessageIsNotValidException(fcmMessage: fcmMessage, innerException: e, additionalInfo: "Message argument was null");
            }
            catch(ArgumentException e)
            {
                throw new NotificationMessageIsNotValidException(fcmMessage: fcmMessage, innerException: e);
            }
            catch(FirebaseMessagingException e)
            {
                //TODO: Handle different kinds of e.MessagingErrorCodes
                throw new CannotSendNotificationException(fcmMessage: fcmMessage, fcmErrorCode: e.MessagingErrorCode, innerException: e);
            }
        }

        public async Task<bool> IsDeviceTokenValid(string deviceToken)
        {
            if(string.IsNullOrEmpty(deviceToken))
                return false;

            try
            {
                await FirebaseMessaging.DefaultInstance.SendAsync(
                    new Message { Token = deviceToken, }, dryRun: true);
            }
            catch(FirebaseMessagingException e)
            {
                if(e.ErrorCode == ErrorCode.InvalidArgument)
                {
                    return false;
                }
            }

            return true;
        }
        
        private void Initialize(IFcmOptionsFactory optionsFactory)
        {
            if(FirebaseMessaging.DefaultInstance != null)
                return;

            var options = optionsFactory.Create();
            
            try
            {
                FirebaseApp.Create(options);

                _logger.LogInformation("FCM initialization has successfully completed");
            }
            catch(ArgumentException)
            {
                //ArgumentException is thrown if Firebase is already initialized according to the documentation
                //So we silently ignore that ArgumentException that's caused by already existing app 
            }
        }
    }
}
