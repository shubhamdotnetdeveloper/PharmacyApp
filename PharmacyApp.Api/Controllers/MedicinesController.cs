
using Microsoft.AspNetCore.Mvc;
using PharmacyApp.Api.DTOs;
using PharmacyApp.Api.Services;

namespace PharmacyApp.Api.Controllers;

[ApiController]
[Route("api/medicines")]
public class MedicinesController : ControllerBase
{
    private readonly MedicineService _service;

    public MedicinesController(MedicineService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? name)
    {
        var medicines = await _service.GetAllAsync(name);
        return Ok(medicines);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var medicine = await _service.GetByIdAsync(id);

        if (medicine is null)
            return NotFound(new { message = "Medicine not found." });

        return Ok(medicine);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateMedicineRequest request)
    {
        try
        {
            var medicine = await _service.AddAsync(request);

            return CreatedAtAction(
                nameof(GetById),
                new { id = medicine.Id },
                medicine);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
