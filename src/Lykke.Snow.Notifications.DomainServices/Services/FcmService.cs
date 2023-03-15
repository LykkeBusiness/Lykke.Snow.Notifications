using System;
using System.Threading.Tasks;
using Common;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Lykke.Snow.Notifications.Domain.Exceptions;
using Lykke.Snow.Notifications.Domain.Model;
using Lykke.Snow.Notifications.Domain.Services;
using Microsoft.Extensions.Logging;

namespace Lykke.Snow.Notifications.DomainServices
{
    public class FcmService : IFcmService
    {
        private readonly ILogger<FcmService> _logger;
        private readonly string _credentialsFilePath;

        public FcmService(ILogger<FcmService> logger, string credentialsFilePath)
        {
            _logger = logger;
            _credentialsFilePath = credentialsFilePath;
        }

        public void CreateApp()
        {
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

        private void ThrowIfCannotInitialize()
        {
            if(string.IsNullOrEmpty(_credentialsFilePath))
                throw new ArgumentNullException(nameof(_credentialsFilePath));
            
            if(!System.IO.File.Exists(_credentialsFilePath))
                throw new CredentialsFileNotFoundException(attemptedPath: _credentialsFilePath);
            
            if(FirebaseMessaging.DefaultInstance != null)
                throw new FirebaseAppAlreadyExistsException();
        }

        public async Task SendNotificationToDevice(NotificationMessage messageArg, string deviceToken)
        {
            ThrowIfNotInitialized();

            var fcmMessage = MapToFcmMessage(messageArg, deviceToken);

            try 
            {
                var response = await FirebaseMessaging.DefaultInstance.SendAsync(fcmMessage);
                _logger.LogInformation("Notification has successfully been sent to the device {Device} {Notification}", deviceToken, messageArg.ToJson());
            }
            catch(ArgumentNullException e)
            {
                throw new NotificationMessageIsNotValidException(notificationMessage: messageArg, fcmMessage: fcmMessage, innerException: e, additionalInfo: "Message argument was null");
            }
            catch(ArgumentException e)
            {
                throw new NotificationMessageIsNotValidException(notificationMessage: messageArg, fcmMessage: fcmMessage, innerException: e);
            }
            catch(FirebaseMessagingException e)
            {
                //TODO: Handle different kinds of e.MessagingErrorCodes
                throw new CannotSendNotificationException(notificationMessage: messageArg, fcmMessage: fcmMessage, fcmErrorCode: e.MessagingErrorCode, innerException: e);
            }
        
        }
        private void ThrowIfNotInitialized()
        {
            if(FirebaseMessaging.DefaultInstance == null)
                throw new NotificationServiceNotInitializedException();
        }
        
        public Message MapToFcmMessage(NotificationMessage notificationMessage, string deviceToken)
        {
            return new Message()
            {
                Token = deviceToken,
                Notification = new Notification()
                {
                    Title = notificationMessage.Title,
                    Body = notificationMessage.Body
                },
                Data = notificationMessage.KeyValueBag
            };
        }
        
        
    }
}