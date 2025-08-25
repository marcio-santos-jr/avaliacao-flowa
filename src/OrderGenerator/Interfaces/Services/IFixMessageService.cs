using OrderGenerator.Models;

namespace OrderGenerator.Interfaces.Services;
public interface IFixMessageService
{
    Task SendNewOrderSingleAsync(Order order);
}
