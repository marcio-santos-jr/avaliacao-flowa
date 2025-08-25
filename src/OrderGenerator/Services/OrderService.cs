using OrderGenerator.Interfaces.Repositories;
using OrderGenerator.Interfaces.Services;
using OrderGenerator.Models;
using OrderGenerator.Models.Enum;
using QuickFix;
using QuickFix.Fields;

namespace OrderGenerator.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger<OrderService> _logger;
    private readonly IFixMessageService _fixMessageService;

    public OrderService(IOrderRepository orderRepository,
                        ILogger<OrderService> logger,
                        IFixMessageService fixMessageService)
    {
        _orderRepository = orderRepository;
        _logger = logger;
        _fixMessageService = fixMessageService;

        _logger.LogInformation($"OrderService inicializado.");
    }

    public async Task<IEnumerable<Order>> GetAllOrders()
    {
        return await _orderRepository.GetAll();
    }

    public async Task<Order> CreateAndSendOrder(string symbol, OrderSide side, decimal quantity, decimal price)
    {
        var newOrder = new Order
        {
            ClOrdID = Guid.NewGuid().ToString(),
            Symbol = symbol,
            Side = side,
            Quantity = quantity,
            Price = price,
            Status = OrderStatus.Pending,
            OrderTime = DateTime.UtcNow
        };

        await _orderRepository.AddOrder(newOrder);

        try
        {
            await _fixMessageService.SendNewOrderSingleAsync(newOrder);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Erro inesperado ao tentar enviar mensagem FIX para ClOrdID '{newOrder.ClOrdID}'.");
            await _orderRepository.UpdateOrderStatus(newOrder.ClOrdID, OrderStatus.Error, $"Erro inesperado no envio FIX: {ex.Message}");
            throw;
        }

        return newOrder;
    }
  
}