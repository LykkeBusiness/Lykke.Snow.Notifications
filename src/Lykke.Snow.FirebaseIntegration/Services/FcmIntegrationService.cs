using System;
using System.Threading.Tasks;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Lykke.Snow.Common.Model;
using Lykke.Snow.FirebaseIntegration.Exceptions;
using Lykke.Snow.FirebaseIntegration.Interfaces;
using Microsoft.Extensions.Logging;

namespace Lykke.Snow.FirebaseIntegration.Services
{
    public class FcmIntegrationService : IFcmIntegrationService {
        private readonly string _credentialsFilePath;

        public FcmIntegrationService(string? credentialsFilePath)
        {
            _credentialsFilePath = credentialsFilePath ?? throw new ArgumentNullException(nameof(credentialsFilePath));

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

            try
            {
                FirebaseApp.Create(new AppOptions() 
                {
                    Credential = GoogleCredential.FromFile(_credentialsFilePath)
                });
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
                var result = await FirebaseMessaging.DefaultInstance.SendAsync(
                    new Message() 
                    { 
                        Token = deviceToken, 
                    }, dryRun: true);
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
    }
}
