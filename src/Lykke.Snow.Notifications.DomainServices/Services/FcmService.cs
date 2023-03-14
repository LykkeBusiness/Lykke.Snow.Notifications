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

        public FcmService(ILogger<FcmService> logger)
        {
            _logger = logger;
        }

        public void CreateApp(string credentialsFilePath)
        {
            if(string.IsNullOrEmpty(credentialsFilePath))
                throw new ArgumentNullException(nameof(credentialsFilePath));
            
            if(!System.IO.File.Exists(credentialsFilePath))
                throw new CredentialsFileNotFoundException(attemptedPath: credentialsFilePath);
            
            if(FirebaseMessaging.DefaultInstance != null)
                throw new FirebaseAppAlreadyExistsException();

            try
            {
                FirebaseApp.Create(new AppOptions() 
                {
                    Credential = GoogleCredential.FromFile(credentialsFilePath)
                });
            }
            catch(ArgumentException e)
            {
                // Documentation states that ArgumentException is thrown 
                // if default app instance already exists
                throw new FirebaseAppAlreadyExistsException(e);
            }
            catch(Exception e)
            {
                throw new FirebaseAppInitializationFailedException(e);
            }
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
                throw new CannotSendNotificationException(notificationMessage: messageArg, fcmMessage: fcmMessage, innerException: e);
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
