using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using FirebaseAdmin.Messaging;
using Lykke.Snow.FirebaseIntegration.Exceptions;
using Lykke.Snow.FirebaseIntegration.Services;
using Lykke.Snow.Notifications.Tests.Model;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Lykke.Snow.Notifications.Tests
{
    class NotificationMessageData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            var message1 = new DummyMessage(title: "Notification title", body: "Notification body");
            var token1 = "device-token-1";
            
            var keyValueBag = new Dictionary<string, string>();
            keyValueBag.Add("key1", "value1");
            keyValueBag.Add("key2", "value2");
            var message2 = new DummyMessage(title: "Notification title 2", body: "Notification body 2", keyValueBag);
            var token2 = "device-token-2";
            
            yield return new object[] { message1, token1 };
            yield return new object[] { message2, token2 };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class FcmIntegrationServiceTests
    {
        [Fact]
        public void CreateApp_ShouldThrowException_IfCredentialsFilePathIsNotProvided()
        {
            var sut = CreateSut();
            
            Assert.Throws<ArgumentNullException>(() => sut.CreateApp());
            Assert.Throws<ArgumentNullException>(() => sut.CreateApp());
        }

        [Fact]
        public async Task SendNotificationToDevice_ShouldThrow_IfFirebaseAppHasNotBeenCreated()
        {
            var sut = CreateSut();
            
            var fcmMessage = new Message();
            
            await Assert.ThrowsAsync<FirebaseAppNotCreatedException>(() => sut.SendNotification(fcmMessage, "any-device-token"));
        }
        
        private FcmIntegrationService CreateSut()
        {
            var mockLogger = new Mock<ILogger<FcmIntegrationService>>();

            return new FcmIntegrationService(mockLogger.Object, credentialsFilePath: string.Empty);
        }
    }
}
