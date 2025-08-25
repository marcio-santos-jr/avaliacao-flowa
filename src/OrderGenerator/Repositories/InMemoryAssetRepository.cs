using OrderGenerator.Interfaces.Repositories;
using OrderGenerator.Models;

namespace OrderGenerator.Repositories;
public class InMemoryAssetRepository : IAssetRepository
{
    public Task<IEnumerable<Asset>> GetAllAsync()
    {
        var listAssets = new List<Asset>()
        {
            new()
            {
                Id = "1",
                Description = "PETR4"
            },
            new()
            {

                Id = "2",
                Description = "VALE3"
            },
            new()
            {

                Id = "3",
                Description = "VIIA4"
            },
        };
        return Task.FromResult<IEnumerable<Asset>>(listAssets);
    }
}
