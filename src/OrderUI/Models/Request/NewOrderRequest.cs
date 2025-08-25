namespace OrderUI.Models.Request;

public class NewOrderRequest
{
    public string? Symbol { get; set; }
    public int Side { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}
