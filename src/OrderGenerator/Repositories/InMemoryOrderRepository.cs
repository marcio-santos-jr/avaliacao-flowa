
using OrderGenerator.Models.Enum;
using OrderGenerator.Models;
using System.Collections.Concurrent;
using OrderGenerator.Interfaces.Repositories;

namespace OrderGenerator.Repositories
{
    public class InMemoryOrderRepository : IOrderRepository
    {
        private readonly ConcurrentDictionary<string, Order> _orders = new ConcurrentDictionary<string, Order>();

        public Task AddOrder(Order order)
        {
            _orders[order.ClOrdID] = order;
            return Task.CompletedTask;
        }

        public Task<Order> GetOrderById(string clOrdId)
        {
            _orders.TryGetValue(clOrdId, out var order);
            return Task.FromResult(order);
        }

        public Task UpdateOrderStatus(string clOrdId, OrderStatus status, string rejectionReason = null)
        {
            if (_orders.TryGetValue(clOrdId, out var order))
            {
                order.Status = status;
                order.RejectionReason = rejectionReason;
            }
            return Task.CompletedTask;
        }

        public Task<IEnumerable<Order>> GetAll()
        {
            return Task.FromResult<IEnumerable<Order>>(_orders.Values.ToList());
        }
    }
}
