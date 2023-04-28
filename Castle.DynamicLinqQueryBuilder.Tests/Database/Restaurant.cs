using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Castle.DynamicLinqQueryBuilder.Tests.Database;


[ExcludeFromCodeCoverage]
public class Restaurant
{
    public ObjectId Id { get; set; }
    public string Name { get; set; }
    [BsonElement("restaurant_id")]
    public string RestaurantId { get; set; }
    public Dictionary<string, object> Details { get; set; }
    [BsonElement("status")]
    public Status Status { get; set; }
}

public enum Status
{
    Open,
    Closed
}