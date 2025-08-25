using OrderGenerator.Interfaces.Repositories;
using OrderGenerator.Interfaces.Services;
using OrderGenerator.Models;

namespace OrderGenerator.Services
{
    public class AssetService : IAssetService
    {
        private IAssetRepository _assetRepository;
        public AssetService(IAssetRepository assetRepository)
        {
            _assetRepository = assetRepository;
        }

        public async Task<IEnumerable<Asset>> GetAllAssets()
        {
            return await _assetRepository.GetAllAsync();
        }
    }
}
