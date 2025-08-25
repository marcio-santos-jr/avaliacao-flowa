using OrderUI.Models.Request;
using OrderUI.Models.Response;
using OrderUI.Models.ViewModel;

namespace OrderUI.Interfaces
{
    public interface IOrderGeneratorService
    {
        Task<IndexViewModel> GetInfoPositionAsync();
        Task<OrderResponse?> CreateOrderAsync(OrderRequest orderRequest);
    }
}
