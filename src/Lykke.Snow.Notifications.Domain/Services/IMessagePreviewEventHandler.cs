using System.Threading.Tasks;
using Lykke.Snow.Notifications.Domain.Model;

namespace Lykke.Snow.Notifications.Domain.Services
{
    /// <summary>
    /// Responsible for handling all MessagePreview events that coming to the Notification Service from Meteor
    /// </summary>
    public interface IMessagePreviewEventHandler 
    {
        Task Handle(MessagePreviewEvent e);
    }
}
