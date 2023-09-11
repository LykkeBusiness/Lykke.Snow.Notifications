using System;
using System.Threading.Tasks;
using Lykke.Snow.Notifications.Domain.Services;
using MarginTrading.AssetService.Contracts;
using Microsoft.Extensions.Caching.Memory;

namespace Lykke.Snow.Notifications.DomainServices.Services
{
    public class AssetService: IAssetService
    {
        private readonly IAssetsApi _assetsApi;
        private readonly IMemoryCache _cache;

        public AssetService(IAssetsApi assetsApi, IMemoryCache cache)
        {
            _assetsApi = assetsApi;
            _cache = cache;
        }

        public async Task<int?> GetContractSize(string assetId)
        {
            var key = $"{nameof(AssetService)}.{nameof(GetContractSize)}.{assetId}";
            return await _cache.GetOrCreateAsync(key, async entity =>
            {
                entity.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1);
                
                var asset = await _assetsApi.GetLegacyAssetById(assetId);
                return asset?.ContractSize;
            });
        }
    }
}
