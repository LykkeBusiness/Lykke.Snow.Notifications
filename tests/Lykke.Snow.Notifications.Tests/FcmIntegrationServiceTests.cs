using System;
using System.Collections;
using System.Collections.Generic;
using Lykke.Snow.FirebaseIntegration.Exceptions;
using Lykke.Snow.FirebaseIntegration.Services;
using Lykke.Snow.Notifications.Domain.Enums;
using Lykke.Snow.Notifications.Domain.Model;
using Xunit;

namespace Lykke.Snow.Notifications.Tests
{
    class NotificationMessageData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            var message1 = new NotificationMessage(title: "Notification title", body: "Notification body", 
                type: NotificationType.AccountLocked,
                new Dictionary<string, string>());

            var token1 = "device-token-1";
            
            var keyValueBag = new Dictionary<string, string>();
            keyValueBag.Add("key1", "value1");
            keyValueBag.Add("key2", "value2");
            var message2 = new NotificationMessage(title: "Notification title 2", body: "Notification body 2",
                NotificationType.CashLocked,
                keyValueBag);

            var token2 = "device-token-2";
            
            yield return new object[] { message1, token1 };
            yield return new object[] { message2, token2 };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class FcmIntegrationServiceTests
    {
        [Fact]
        public void InstantiatingWithNullCredentialsPath_ShouldThrow_ArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => CreateSut(null));
        }

        [Fact]
        public void InstantiatingWithInvalidCredentialsPath_ShouldThrow_FirebaseCredentialsFileNotFoundException()
        {
            Assert.Throws<FirebaseCredentialsFileNotFoundException>(() => CreateSut("any-credentials-path"));
        }
        
        private FcmIntegrationService CreateSut(string credentialsFilePath)
        {
            if(credentialsFilePath == null)
            {
                return new FcmIntegrationService(credentialsFilePath: credentialsFilePath);
            }

            return new FcmIntegrationService(credentialsFilePath: string.Empty);
        }
    }
}
