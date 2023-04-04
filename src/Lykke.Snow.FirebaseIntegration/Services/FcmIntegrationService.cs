using System;
using System.Threading.Tasks;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Lykke.Snow.Common.Model;
using Lykke.Snow.FirebaseIntegration.Exceptions;
using Lykke.Snow.FirebaseIntegration.Interfaces;

namespace Lykke.Snow.FirebaseIntegration.Services
{
    /// <summary>
    /// Firebase Cloud Messaging client. Communicates to Firebase services.
    /// </summary>
    public class FcmIntegrationService : IFcmIntegrationService 
    {
        private readonly string _credentialsFilePath;
        private readonly ProxyConfiguration? _proxyConfiguration;

        public FcmIntegrationService(string? credentialsFilePath, ProxyConfiguration? proxyConfiguration = null)
        {
            _credentialsFilePath = credentialsFilePath ?? throw new ArgumentNullException(nameof(credentialsFilePath));
            _proxyConfiguration = proxyConfiguration;

            if (!System.IO.File.Exists(_credentialsFilePath))
                throw new FirebaseCredentialsFileNotFoundException(attemptedPath: _credentialsFilePath);

            Initialize();
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

        private void Initialize()
        {
            if(FirebaseMessaging.DefaultInstance != null)
                return;

            var options = CreateAppOptions();
            
            try
            {
                FirebaseApp.Create(options);
            }
            catch(ArgumentException)
            {
                //ArgumentException is thrown if Firebase is already initialized according to the documentation
                //So we silently ignore that ArgumentException that's caused by already existing app 
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
        
        private AppOptions CreateAppOptions()
        {
            var options = new AppOptions
            {
                Credential = GoogleCredential.FromFile(_credentialsFilePath)
            };

            if (_proxyConfiguration != null)
            {
                options.HttpClientFactory = new HttpClientFactoryWithProxy(_proxyConfiguration.Value);
            }

            return options;
        }
    }
}
