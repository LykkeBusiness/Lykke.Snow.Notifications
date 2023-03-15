using System;
using System.Collections;
using System.Collections.Generic;
using Lykke.Snow.Notifications.Domain.Model;
using Lykke.Snow.Notifications.DomainServices;
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

    public class FcmServiceTests
    {
        [Fact]
        public void CreateApp_ShouldThrowException_IfCredentialsFilePathIsNotProvided()
        {
            var sut = CreateSut();
            
            Assert.Throws<ArgumentNullException>(() => sut.CreateApp());
            Assert.Throws<ArgumentNullException>(() => sut.CreateApp());
        }
        
        [Theory]
        [ClassData(typeof(NotificationMessageData))]
        public void MappingNotificationMessage_ToFcmMessage(NotificationMessage notificationMessage, string deviceToken)
        {
            var sut = CreateSut();
            
            var fcmMessage = sut.MapToFcmMessage(notificationMessage, deviceToken);
            
            Assert.Equal(expected: notificationMessage.Title, actual: fcmMessage.Notification.Title);
            Assert.Equal(expected: notificationMessage.Body, actual: fcmMessage.Notification.Body);
            Assert.Equal(expected: notificationMessage.KeyValueBag, actual: fcmMessage.Data);
        }

        
        private FcmService CreateSut()
        {
            var mockLogger = new Mock<ILogger<FcmService>>();

            return new FcmService(mockLogger.Object, credentialsFilePath: string.Empty);
        }
    }
}
