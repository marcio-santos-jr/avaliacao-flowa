using Microsoft.AspNetCore.Mvc.Rendering;
using OrderUI.Clients;
using OrderUI.Interfaces;
using OrderUI.Models.Enum;
using OrderUI.Models.Request;
using OrderUI.Models.Response;
using OrderUI.Models.ViewModel;

namespace OrderUI.Services;
public class OrderGeneratorService(IOrderGeneratorApi orderGeneratorApi) : IOrderGeneratorService
{
    private readonly IOrderGeneratorApi _orderGeneratorApi = orderGeneratorApi;

    public async Task<IndexViewModel> GetInfoPositionAsync()
    {
        var orders = await _orderGeneratorApi.GetAllOrdersAsync();
        var assets = await _orderGeneratorApi.GetAllAssets();

        var posicoes = orders.Where(x => x.Status == OrderStatus.Accepted).GroupBy(x => x.Symbol).Select(x => new PositionSummary()
        {
            Exposure = x.Where(x => x.Side == OrderSide.Buy).Sum(x => x.Quantity * x.Price) - x.Where(x => x.Side == OrderSide.Sell).Sum(x => x.Quantity * x.Price),
            Quantity = x.Where(x => x.Side == OrderSide.Buy).Sum(x => x.Quantity) - x.Where(x => x.Side == OrderSide.Sell).Sum(x => x.Quantity),
            Symbol = x.Key
        }).ToList();


        return new IndexViewModel()
        {
            Orders = orders,
            PositionSummary = posicoes,
            AvailableSymbols = assets.Select(x => new SelectListItem { Text = x.Description, Value = x.Description })
        };
    }

    public async Task<OrderResponse?> CreateOrderAsync(OrderRequest orderRequest)
    {
        var newOrder = new NewOrderRequest()
        {
            Price = orderRequest.Price,
            Quantity = orderRequest.Quantity,
            Side = (int)orderRequest.Side,
            Symbol = orderRequest.Symbol
        };
        return await _orderGeneratorApi.CreateOrderAsync(newOrder);
    }
}