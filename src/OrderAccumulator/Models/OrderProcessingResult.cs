using OrderAccumulator.Models.Enum;

namespace OrderAccumulator.Models
{
    public class OrderProcessingResult
    {
        public bool IsAccepted { get; set; }
        public string ClOrdID { get; set; }
        public string Symbol { get; set; } 
        public OrderSide Side { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public string RejectionReason { get; set; }
        public decimal? NewProjectedExposure { get; set; } 
    }
}