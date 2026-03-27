using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Lab2.Entities;

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
    [BsonRepresentation(BsonType.String)]
    public InsuranceCategory Category { get; set; }

    [BsonElement("amount")]
    public decimal Amount { get; set; }

    [BsonElement("contractDate")]
    public DateTime ContractDate { get; set; } = DateTime.UtcNow;
}