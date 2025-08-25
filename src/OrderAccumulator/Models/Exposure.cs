namespace OrderAccumulator.Models;

public class Exposure
{
    public required string Symbol { get; set; }
    public decimal CurrentExposureAmount { get; set; }
}
