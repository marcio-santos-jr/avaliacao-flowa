using OrderUI.Models.Request;
using OrderUI.Models.Response;
using Refit;

namespace OrderUI.Clients
{
    public interface IOrderGeneratorApi
    {
        [Get("/Orders")]
        Task<List<OrderResponse>> GetAllOrdersAsync();

        [Post("/Orders")]
        Task<OrderResponse> CreateOrderAsync([Body] NewOrderRequest request);

        [Get("/Assets")]
        Task<List<AssetResponse>> GetAllAssets();
    }
}
