using System.Threading.Tasks;

namespace Lykke.Snow.Notifications.Domain.Services
{
    public interface IAssetService
    {
        Task<int?> GetContractSize(string assetId);
    }
}
