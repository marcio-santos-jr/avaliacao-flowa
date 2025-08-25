using OrderAccumulator.Models;

namespace OrderAccumulator.Interfaces.Service;

public interface IOrderService
{
    Task<OrderProcessingResult> ProcessOrderAsync(Order order);
}
