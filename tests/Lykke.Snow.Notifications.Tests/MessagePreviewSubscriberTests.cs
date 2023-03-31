using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Snow.Notifications.Domain.Model;
using Lykke.Snow.Notifications.Domain.Services;
using Lykke.Snow.Notifications.Settings;
using Lykke.Snow.Notifications.Subscribers;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Lykke.Snow.Notifications.Tests
{
    class MessagePreviewEventTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[]
            {
                new MessagePreviewEvent
                {
                    Recipients = new[] { "A01", "A02", "A03" },
                    Subject = "some-subject",
                    Content = "some-content"
                }
            };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class MessagePreviewSubscriberTests
    {
        [Theory]
        [ClassData(typeof(MessagePreviewEventTestData))]
        public async Task ProcessMessageAsync_ShouldPassTheEvent_ToMessagePreviewEventHandler(MessagePreviewEvent e)
        {
            var mockEventHandler = new Mock<IMessagePreviewEventHandler>();

            var sut = CreateSut(mockEventHandler.Object);
            
            await sut.ProcessMessageAsync(e);
            
            mockEventHandler.Verify(x => x.Handle(e), Times.Once);
        }
        
        private MessagePreviewSubscriber CreateSut(IMessagePreviewEventHandler? messagePreviewEventHandlerArg = null)
        {
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var subscriptionSettings = new SubscriptionSettings();
            
            IMessagePreviewEventHandler messagePrevieEventHandler = new Mock<IMessagePreviewEventHandler>().Object;
            
            if(messagePreviewEventHandlerArg != null)
            {
                messagePrevieEventHandler = messagePreviewEventHandlerArg;
            }

            return new MessagePreviewSubscriber(mockLoggerFactory.Object, subscriptionSettings, messagePrevieEventHandler);
        }
    }
}
