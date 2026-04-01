using Lab2.DTOs;
using Lab2.Entities;
using Lab2.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lab2.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InsuranceContractController : ControllerBase
{
    private readonly InsuranceContractService _service;

    public InsuranceContractController(InsuranceContractService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var contracts = await _service.GetAllAsync();
        return Ok(contracts);
    }

    [HttpGet("filter")]
    public async Task<IActionResult> Filter(
        [FromQuery] string? contractNumber,
        [FromQuery] string? clientIdentity,
        [FromQuery] string? objectIdentity,
        [FromQuery] string? category,
        [FromQuery] decimal? minAmount,
        [FromQuery] decimal? maxAmount,
        [FromQuery] DateTime? contractDateFrom,
        [FromQuery] DateTime? contractDateTo)
    {
        InsuranceCategory? queriedCategory = null;
        if (category != null)
        {
            if (Enum.TryParse<InsuranceCategory>(category, out InsuranceCategory result)) queriedCategory = result;
        }
        var contracts = await _service.FilterAsync(
        contractNumber, clientIdentity, objectIdentity,
        queriedCategory, minAmount, maxAmount,
        contractDateFrom, contractDateTo);
        return Ok(contracts);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var contract = await _service.GetByIdAsync(id);
        if (contract == null)
            return NotFound();
        return Ok(contract);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] InsuranceContractRequestDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var contract = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = contract.Id }, contract);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] InsuranceContractRequestDto dto)
    {
        var updated = await _service.UpdateAsync(id, dto);
        if (!updated)
            return NotFound();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted)
            return NotFound();
        return NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteAll()
    {
        var deletedCount = await _service.DeleteAllAsync();
        return NoContent();
    }
}