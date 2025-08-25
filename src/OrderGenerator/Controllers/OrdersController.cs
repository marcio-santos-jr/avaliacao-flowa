using Microsoft.AspNetCore.Mvc;
using OrderGenerator.Interfaces.Services;
using OrderGenerator.Models;

namespace OrderGenerator.Controllers;

[ApiController]
[Route("[controller]")]
public class OrdersController(IOrderService orderService, ILogger<OrdersController> logger) : ControllerBase
{
    private readonly IOrderService _orderService = orderService;
    private readonly ILogger<OrdersController> _logger = logger;

    /// <summary>
    /// Cria uma nova ordem e a envia via FIX.
    /// </summary>
    /// <param name="request">Dados da ordem a ser criada.</param>
    /// <returns>A ordem criada com seu status inicial.</returns>
    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] OrderRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var order = await _orderService.CreateAndSendOrder(request.Symbol, request.Side, request.Quantity, request.Price);
            return Created(nameof(GetAllOrders), new { id = order.ClOrdID, order });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar a requisição de criação de ordem.");
            return StatusCode(500, "Erro interno ao criar a ordem.");
        }
    }

    /// <summary>
    /// Lista o histórico de todas as ordens.
    /// </summary>
    /// <returns>Uma lista de todas as ordens registradas.</returns>
    [HttpGet]
    public async Task<IActionResult> GetAllOrders()
    {
        var orders = await _orderService.GetAllOrders();
        return Ok(orders);
    }
}
