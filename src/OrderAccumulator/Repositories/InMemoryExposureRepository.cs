using OrderAccumulator.Interfaces.Repositories;
using OrderAccumulator.Models;
using System.Collections.Concurrent;

namespace OrderAccumulator.Repositories
{
    public class InMemoryExposureRepository(ILogger<InMemoryExposureRepository> logger) : IExposureRepository
    {
        private readonly ConcurrentDictionary<string, Exposure> _exposureData = new();
        private readonly ILogger<InMemoryExposureRepository> _logger = logger;

        public Task<Exposure> GetExposureBySymbol(string symbol)
        {
            _exposureData.TryGetValue(symbol, out var exposure);
            return Task.FromResult(exposure ?? new Exposure { Symbol = symbol, CurrentExposureAmount = 0 });
        }

        public Task UpdateExposure(string symbol, decimal changeAmount)
        {
            _exposureData.AddOrUpdate(symbol,
            addValueFactory: s => new Exposure { Symbol = s, CurrentExposureAmount = changeAmount },
            updateValueFactory: (s, existing) =>
            {
                existing.CurrentExposureAmount += changeAmount;
                return existing;
            });
            _logger.LogInformation($"Exposição para {symbol} atualizada em {changeAmount}. Nova exposição total: {_exposureData[symbol].CurrentExposureAmount}");
            return Task.CompletedTask;
        }

        public Task InitializeSymbolExposure(string symbol)
        {
            _exposureData.TryAdd(symbol, new Exposure { Symbol = symbol, CurrentExposureAmount = 0 });
            return Task.CompletedTask;
        }
    }
}
