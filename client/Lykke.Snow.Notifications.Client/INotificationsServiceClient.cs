using JetBrains.Annotations;

namespace Lykke.Snow.Notifications.Client
{
    /// <summary>
    /// Notifications API client interface.
    /// </summary>
    [PublicAPI]
    public interface INotificationsServiceClient
    {
        /// <summary>Notifications Api interface</summary>
        INotificationsApi Api { get; }
    }
}
