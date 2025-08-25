using OrderGenerator.Models.Enum;
using OrderGenerator.Models;

namespace OrderGenerator.Interfaces.Repositories;

public interface IOrderRepository
{
    Task AddOrder(Order order);
    Task UpdateOrderStatus(string id, OrderStatus status, string? rejectionReason);
    Task<IEnumerable<Order>> GetAll();
}
