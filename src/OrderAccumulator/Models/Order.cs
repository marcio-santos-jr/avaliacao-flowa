using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using OrderAccumulator.Models.Enum;
namespace OrderAccumulator.Models;

public class Order
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public required string ClOrdID { get; set; }

    [BsonElement("Symbol")]
    public required string Symbol { get; set; }

    [BsonElement("Side")]
    public OrderSide Side { get; set; }

    [BsonElement("Quantity")]
    public decimal Quantity { get; set; }

    [BsonElement("Price")]
    public decimal Price { get; set; }

    [BsonElement("Status")]
    public OrderStatus Status { get; set; }

    [BsonElement("OrderTime")]
    public DateTime OrderTime { get; set; }

    [BsonElement("UpdatedAt")]
    public DateTime? UpdatedAt { get; set; }

    [BsonElement("RejectionReason")]
    public string? RejectionReason
    {
        get; set;
    }
}