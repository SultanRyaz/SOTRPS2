using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace Lab2.Entities;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum InsuranceCategory
{
    Apartment,
    Auto
}

public class InsuranceContractEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("contractNumber")]
    public string ContractNumber { get; set; } = string.Empty;

    [BsonElement("clientIdentity")]
    public string ClientIdentity { get; set; } = string.Empty;

    [BsonElement("objectIdentity")]
    public string ObjectIdentity { get; set; } = string.Empty;

    [BsonElement("category")]
    public InsuranceCategory Category { get; set; } = InsuranceCategory.Apartment;

    [BsonElement("amount")]
    public decimal Amount { get; set; }

    [BsonElement("contractDate")]
    public DateTime ContractDate { get; set; } = DateTime.UtcNow;
}