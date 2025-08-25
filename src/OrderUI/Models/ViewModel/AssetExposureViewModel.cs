namespace OrderUI.Models.ViewModel;

public class AssetExposureViewModel
{
    public string Symbol { get; set; }
    public decimal Exposure { get; set; }
    public int Quantity { get; set; }
    public DateTime LastUpdate { get; set; }
}