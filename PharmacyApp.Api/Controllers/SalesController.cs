
using Microsoft.AspNetCore.Mvc;
using PharmacyApp.Api.DTOs;
using PharmacyApp.Api.Services;

namespace PharmacyApp.Api.Controllers;

[ApiController]
[Route("api/sales")]
public class SalesController : ControllerBase
{
    private readonly SaleService _service;

    public SalesController(SaleService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var sales = await _service.GetAllAsync();
        return Ok(sales);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateSaleRequest request)
    {
        try
        {
            var sale = await _service.CreateAsync(request);

            return Created(
                $"/api/sales/{sale.Id}",
                sale);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }
}
