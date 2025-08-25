using OrderAccumulator.Models;

namespace OrderAccumulator.Interfaces.Repositories
{
    public interface IExposureRepository
    {
        Task<Exposure> GetExposureBySymbol(string symbol);
        Task UpdateExposure(string symbol, decimal amountToAdd);
        Task InitializeSymbolExposure(string symbol);
    }
}
