using OrderGenerator.Models;

namespace OrderGenerator.Interfaces.Repositories;

public interface IAssetRepository
{
    Task<IEnumerable<Asset>> GetAllAsync();
}