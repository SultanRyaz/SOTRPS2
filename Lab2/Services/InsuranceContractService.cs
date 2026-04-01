using Lab2.Entities;
using Lab2.DTOs;
using Lab2.Models;
using MongoDB.Driver;
using MongoDB.Bson;
using Microsoft.Extensions.Options;

namespace Lab2.Services;

public class InsuranceContractService
{
    private readonly IMongoCollection<InsuranceContractEntity> _collection;

    public InsuranceContractService(IOptions<InsuranceDatabaseSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.DatabaseName);
        _collection = database.GetCollection<InsuranceContractEntity>(settings.Value.CollectionName);
    }

    public async Task<List<InsuranceContractResponseDto>> GetAllAsync()
    {
        var count = await _collection.CountDocumentsAsync(FilterDefinition<InsuranceContractEntity>.Empty);

        List<InsuranceContractEntity> entities;

        if (count > 1000)
        {
            entities = await _collection.Aggregate()
                .Sample(1000)
                .ToListAsync();
        }
        else
        {
            entities = await _collection.Find(_ => true).ToListAsync();
        }

        return entities.Select(e => MapToResponseDto(e)).ToList();
    }

    public async Task<List<InsuranceContractResponseDto>> FilterAsync(
        string? contractNumber = null,
        string? clientIdentity = null,
        string? objectIdentity = null,
        InsuranceCategory? category = null,
        decimal? minAmount = null,
        decimal? maxAmount = null,
        DateTime? contractDateFrom = null,
        DateTime? contractDateTo = null)
    {
        var filterBuilder = Builders<InsuranceContractEntity>.Filter;
        var filters = new List<FilterDefinition<InsuranceContractEntity>>();

        if (!string.IsNullOrEmpty(contractNumber))
            filters.Add(filterBuilder.Eq(x => x.ContractNumber, contractNumber));

        if (!string.IsNullOrEmpty(clientIdentity))
            filters.Add(filterBuilder.Regex(x => x.ClientIdentity, new BsonRegularExpression(clientIdentity, "i")));

        if (!string.IsNullOrEmpty(objectIdentity))
            filters.Add(filterBuilder.Regex(x => x.ObjectIdentity, new BsonRegularExpression(objectIdentity, "i")));

        if (category != null)
            filters.Add(filterBuilder.Eq(x => x.Category, category));

        if (minAmount.HasValue)
            filters.Add(filterBuilder.Gte(x => x.Amount, minAmount.Value));

        if (maxAmount.HasValue)
            filters.Add(filterBuilder.Lte(x => x.Amount, maxAmount.Value));

        if (contractDateFrom.HasValue)
            filters.Add(filterBuilder.Gte(x => x.ContractDate, contractDateFrom.Value));

        if (contractDateTo.HasValue)
            filters.Add(filterBuilder.Lte(x => x.ContractDate, contractDateTo.Value));

        var finalFilter = filters.Any()
            ? filterBuilder.And(filters)
            : FilterDefinition<InsuranceContractEntity>.Empty;

        var entities = await _collection.Find(finalFilter).ToListAsync();
        return entities.Select(e => MapToResponseDto(e)).ToList();
    }

    public async Task<InsuranceContractResponseDto?> GetByIdAsync(string id)
    {
        if (!ObjectId.TryParse(id, out _))
            return null;

        var entity = await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();
        return entity != null ? MapToResponseDto(entity) : null;
    }

    public async Task<InsuranceContractResponseDto> CreateAsync(InsuranceContractRequestDto dto)
    {
        var entity = new InsuranceContractEntity
        {
            ContractNumber = dto.ContractNumber,
            ClientIdentity = dto.ClientIdentity,
            ObjectIdentity = dto.ObjectIdentity,
            Category = dto.Category,
            Amount = dto.Amount,
            ContractDate = dto.ContractDate
        };

        await _collection.InsertOneAsync(entity);
        return MapToResponseDto(entity);
    }

    public async Task<bool> UpdateAsync(string id, InsuranceContractRequestDto dto)
    {
        if (!ObjectId.TryParse(id, out _))
            return false;


        var updateBuilder = Builders<InsuranceContractEntity>.Update;
        var updates = new List<UpdateDefinition<InsuranceContractEntity>>();

        if (!string.IsNullOrEmpty(dto.ContractNumber))
            updates.Add(updateBuilder.Set(x => x.ContractNumber, dto.ContractNumber));

        if (!string.IsNullOrEmpty(dto.ClientIdentity))
            updates.Add(updateBuilder.Set(x => x.ClientIdentity, dto.ClientIdentity));

        if (!string.IsNullOrEmpty(dto.ObjectIdentity))
            updates.Add(updateBuilder.Set(x => x.ObjectIdentity, dto.ObjectIdentity));

        updates.Add(updateBuilder.Set(x => x.Category, dto.Category));

        if (dto.Amount != 0)
            updates.Add(updateBuilder.Set(x => x.Amount, dto.Amount));

        if (dto.ContractDate != default)
            updates.Add(updateBuilder.Set(x => x.ContractDate, dto.ContractDate));

        if (!updates.Any())
            return false;

        var result = await _collection.UpdateOneAsync(
            x => x.Id == id,
            updateBuilder.Combine(updates));

        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        if (!ObjectId.TryParse(id, out _))
            return false;

        var result = await _collection.DeleteOneAsync(x => x.Id == id);
        return result.DeletedCount > 0;
    }

    public async Task<long> DeleteAllAsync()
    {
        var result = await _collection.DeleteManyAsync(_ => true);
        return result.DeletedCount;
    }

    private InsuranceContractResponseDto MapToResponseDto(InsuranceContractEntity entity)
    {
        return new InsuranceContractResponseDto
        {
            Id = entity.Id!,
            ContractNumber = entity.ContractNumber,
            ClientIdentity = entity.ClientIdentity,
            ObjectIdentity = entity.ObjectIdentity,
            Category = entity.Category,
            Amount = entity.Amount,
            ContractDate = entity.ContractDate
        };
    }
}