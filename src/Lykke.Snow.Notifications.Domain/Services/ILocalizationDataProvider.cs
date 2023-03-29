using System.Threading.Tasks;
using Lykke.Snow.Notifications.Domain.Model;

namespace Lykke.Snow.Notifications.Domain.Services
{
    public interface ILocalizationDataProvider
    {
        Task<LocalizationData> Load();
    }
}
