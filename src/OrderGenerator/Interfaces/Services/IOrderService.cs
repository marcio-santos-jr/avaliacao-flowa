using OrderGenerator.Models;
using OrderGenerator.Models.Enum;

namespace OrderGenerator.Interfaces.Services;

public interface IOrderService
{
    Task<IEnumerable<Order>> GetAllOrders();
    Task<Order> CreateAndSendOrder(string symbol, OrderSide side, decimal quantity, decimal price);
}
