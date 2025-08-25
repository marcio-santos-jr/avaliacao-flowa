using Microsoft.AspNetCore.Mvc;
using OrderGenerator.Interfaces.Services;

namespace OrderGenerator.Controllers;

[ApiController]
[Route("[controller]")]
public class AssetsController(IAssetService assetService) : ControllerBase
{
    private readonly IAssetService _assetService = assetService;

    /// <summary>
    /// Lista ativos disponíveis.
    /// </summary>
    /// <returns>Uma lista de todas os ativos registrados.</returns>
    [HttpGet]
    public async Task<IActionResult> GetAllOrders()
    {
        var assets = await _assetService.GetAllAssets();
        return Ok(assets);
    }
}
