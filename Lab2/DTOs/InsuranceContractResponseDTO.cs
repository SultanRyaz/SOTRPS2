using Lab2.Entities;

namespace Lab2.DTOs;

public class InsuranceContractResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string ContractNumber { get; set; } = string.Empty;
    public string ClientIdentity { get; set; } = string.Empty;
    public string ObjectIdentity { get; set; } = string.Empty;
    public InsuranceCategory Category { get; set; }
    public decimal Amount { get; set; }
    public DateTime ContractDate { get; set; }
}