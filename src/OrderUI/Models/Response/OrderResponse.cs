using OrderUI.Models.Enum;

namespace OrderUI.Models.Response;
public class OrderResponse
{
    public string Symbol { get; set; }
    public OrderSide Side { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public OrderStatus Status { get; set; }
    public DateTime OrderTime { get; set; }
    public string? RejectionReason { get; set; }
}
