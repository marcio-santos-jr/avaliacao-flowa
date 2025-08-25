namespace OrderUI.Models.ViewModel;

public class PositionSummary
{
    public string Symbol { get; set; }
    public int Quantity { get; set; }
    public decimal Exposure { get; set; }
    public DateTime LastUpdate { get; } = DateTime.Now;
}
