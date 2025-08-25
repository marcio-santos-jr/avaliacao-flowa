using Microsoft.Extensions.Caching.Memory;
using MongoDB.Driver;
using OrderAccumulator.Interfaces.Repositories;
using OrderAccumulator.Models;
using OrderAccumulator.Models.Enum;
using OrderAccumulator.Repositories.Context;

namespace OrderAccumulator.Repositories
{
    public class ExposureRepository(MongoDbContext dbContext, IMemoryCache cache) : IExposureRepository
    {
        private readonly IMongoCollection<Order> _collection = dbContext.GetCollection<Order>();
        private readonly IMemoryCache _cache = cache;
        private static readonly string CachePrefix = "exposure-";

        public async Task<Exposure> GetExposureBySymbol(string symbol)
        {
            if (_cache.TryGetValue($"{CachePrefix}{symbol}", out Exposure exposure))
                return exposure;

            await InitializeSymbolExposure(symbol);
            return _cache.Get<Exposure>($"{CachePrefix}{symbol}")!;
        }

        public async Task InitializeSymbolExposure(string symbol)
        {
            var orders = await _collection.Find(o =>
                o.Symbol == symbol &&
                o.Status == OrderStatus.Accepted
            ).ToListAsync();

            decimal exposureAmount = orders.Sum(o =>
                (int)o.Side == (int)OrderSide.Buy ? (o.Quantity * o.Price) : -(o.Quantity * o.Price));

            var exposure = new Exposure
            {
                Symbol = symbol,
                CurrentExposureAmount = exposureAmount
            };

            _cache.Set($"{CachePrefix}{symbol}", exposure,
                new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                });
        }

        public async Task UpdateExposure(string symbol, decimal changeAmount)
        {
            if (!_cache.TryGetValue($"{CachePrefix}{symbol}", out Exposure exposure))
            {
                await InitializeSymbolExposure(symbol);
                exposure = _cache.Get<Exposure>($"{CachePrefix}{symbol}")!;
            }

            exposure.CurrentExposureAmount += changeAmount;

            _cache.Set($"{CachePrefix}{symbol}", exposure,
                new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                });
        }
    }
}
