using System.Net.Http.Json;
using Lab2.DTOs;
using Lab2.Entities;
using Lab2.Tests.Factories;

namespace Lab2.Tests;

public class InsuranceContractApiTest : IClassFixture<ApiFactory>
{
    private readonly HttpClient httpClient;

    public InsuranceContractApiTest(ApiFactory factory)
    {
        httpClient = factory.CreateClient();
    }

    [Fact]
    public async Task Test_Add_100_Elements_One_By_One()
    {
        var dtos = Generate(100);

        foreach (var dto in dtos)
        {
            var response = await httpClient.PostAsJsonAsync("api/InsuranceContract", dto);
            response.EnsureSuccessStatusCode();
        }
    }

    [Fact]
    public async Task Test_Add_100000_Elements_One_By_One()
    {
        var dtos = Generate(100000);

        foreach (var dto in dtos)
        {
            var response = await httpClient.PostAsJsonAsync("api/InsuranceContract", dto);
            response.EnsureSuccessStatusCode();
        }
    }

    [Fact]
    public async Task Test_Delete_All()
    {
        var dtos = Generate(100);

        foreach (var dto in dtos)
        {
            var response = await httpClient.PostAsJsonAsync("api/InsuranceContract", dto);
            response.EnsureSuccessStatusCode();
        }

        var responseDelete = await httpClient.DeleteAsync("api/InsuranceContract");
        responseDelete.EnsureSuccessStatusCode();
    }

    private List<InsuranceContractRequestDto> Generate(int count)
    {
        var random = new Random();
        var batch = new List<InsuranceContractRequestDto>();
        var categories = Enum.GetValues<InsuranceCategory>();

        for (int i = 0; i < count; i++)
        {
            batch.Add(new InsuranceContractRequestDto
            {
                ContractNumber = $"{i+1}",
                ClientIdentity = $"{Guid.NewGuid():N}".Substring(0, 12),
                ObjectIdentity = $"{Guid.NewGuid():N}".Substring(0, 12),
                Category = categories[random.Next(categories.Length)],
                Amount = random.Next(10_000, 1_000_000),
                ContractDate = DateTime.UtcNow.AddDays(-random.Next(0, 365))
            });
        }

        return batch;
    }
}