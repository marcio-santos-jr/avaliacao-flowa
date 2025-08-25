using Microsoft.AspNetCore.Mvc;
using OrderUI.Interfaces;
using OrderUI.Models.Request;

namespace OrderUI.Controllers;
public class HomeController(IOrderGeneratorService orderGeneratorService) : Controller
{
    private readonly IOrderGeneratorService _orderGeneratorService = orderGeneratorService;

    public async Task<IActionResult> Index()
    {
       var result = await _orderGeneratorService.GetInfoPositionAsync();
       return View(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] OrderRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                );

            return Json(new
            {
                success = false,
                message = "Dados da ordem inválidos.",
                errors
            });
        }


        var result = await _orderGeneratorService.CreateOrderAsync(request);
        if(result != null)
            return Json(new { success = true, message = "Ordem enviada com sucesso!", order = result });
        else
            return Json(new { success = false, message = $"Erro inesperado ao enviar a ordem" });
    }

}