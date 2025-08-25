using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace OrderGenerator.Models;

public class Asset
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("Description")]
    public required string Description { get; set; }
}
