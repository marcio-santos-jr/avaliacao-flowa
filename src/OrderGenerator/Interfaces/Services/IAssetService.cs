using OrderGenerator.Models;

namespace OrderGenerator.Interfaces.Services;

public interface IAssetService
{
    Task<IEnumerable<Asset>> GetAllAssets();
}
