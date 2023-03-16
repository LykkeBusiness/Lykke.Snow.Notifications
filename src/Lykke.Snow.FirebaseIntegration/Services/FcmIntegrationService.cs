using System;
using System.Threading.Tasks;
using Common;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Lykke.Snow.FirebaseIntegration.Exceptions;
using Lykke.Snow.FirebaseIntegration.Interfaces;
using Lykke.Snow.FirebaseIntegration.Model;
using Microsoft.Extensions.Logging;

namespace Lykke.Snow.FirebaseIntegration.Services
{
    public class FcmIntegrationService : IFcmIntegrationService
    {
        private readonly ILogger<FcmIntegrationService> _logger;
        private readonly string _credentialsFilePath;

        public FcmIntegrationService(ILogger<FcmIntegrationService> logger, string credentialsFilePath)
        {
            _logger = logger;
            _credentialsFilePath = credentialsFilePath;
            
            Initialize();
        }

        private void ThrowIfCannotInitialize()
        {
            if(string.IsNullOrEmpty(_credentialsFilePath))
                throw new ArgumentNullException(nameof(_credentialsFilePath));
            
            if(!System.IO.File.Exists(_credentialsFilePath))
                throw new FirebaseCredentialsFileNotFoundException(attemptedPath: _credentialsFilePath);
        }

        public async Task<SendNotificationResult> SendNotification(Message fcmMessage, string deviceToken)
        {
            ThrowIfNotInitialized();

            try 
            {
                var response = await FirebaseMessaging.DefaultInstance.SendAsync(fcmMessage);
                _logger.LogInformation("Notification has successfully been sent to the device {Device} {Notification}", deviceToken, fcmMessage.ToJson());

                return SendNotificationResult.Success(messageId: response);
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

        private void Initialize()
        {
            if(FirebaseMessaging.DefaultInstance != null)
                return;

            ThrowIfCannotInitialize();

            try
            {
                FirebaseApp.Create(new AppOptions() 
                {
                    Credential = GoogleCredential.FromFile(_credentialsFilePath)
                });
            }
            catch(ArgumentException e)
            {
                throw new FirebaseAppAlreadyExistsException(e);
            }
            catch(Exception e)
            {
                throw new FirebaseAppInitializationFailedException(e);
            }
        }

        private void ThrowIfNotInitialized()
        {
            if(FirebaseMessaging.DefaultInstance == null)
                throw new FirebaseAppNotCreatedException();
        }
    }
}
