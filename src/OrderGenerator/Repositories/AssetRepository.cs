using MongoDB.Driver;
using OrderGenerator.Interfaces.Repositories;
using OrderGenerator.Models;
using OrderGenerator.Repositories.Context;

namespace OrderGenerator.Repositories;
public class AssetRepository : IAssetRepository
{
    private readonly IMongoCollection<Asset> _collection;

    public AssetRepository(MongoDbContext dbContext)
    {
        _collection = dbContext.GetCollection<Asset>();

        var indexKeys = Builders<Asset>.IndexKeys.Ascending(a => a.Description);
        var indexOptions = new CreateIndexOptions { Name = "AssetDescriptionUniqueIndex", Unique = true };

        _collection.Indexes.CreateOneAsync(new CreateIndexModel<Asset>(indexKeys, indexOptions)).Wait();

        var assetsToSeed = new List<string> { "PETR4", "VALE3", "VIIA4" };

        foreach (var symbol in assetsToSeed)
        {
            Task.Run(async () =>
            {
                var filter = Builders<Asset>.Filter.Eq(a => a.Description, symbol);
                var existingAsset = await _collection.Find(filter).FirstOrDefaultAsync();

                if (existingAsset == null)
                {
                    var newAsset = new Asset()
                    {
                        Description = symbol
                    };
                    await _collection.InsertOneAsync(newAsset);
                }
            }).Wait();
        }
    }

    public async Task<IEnumerable<Asset>> GetAllAsync() =>
        await _collection.Find(_ => true).ToListAsync();


}
