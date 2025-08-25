using Microsoft.AspNetCore.Mvc.Rendering;
using OrderUI.Models.Response;

namespace OrderUI.Models.ViewModel;
public class IndexViewModel
{
    public List<PositionSummary> PositionSummary { get; set; } = [];
    public List<OrderResponse> Orders { get; set; } = [];

    public IEnumerable<SelectListItem> AvailableSymbols { get; set; } = [];
}
