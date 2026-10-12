
using System.ComponentModel.DataAnnotations;

namespace PharmacyApp.Api.DTOs;

public class CreateMedicineRequest
{
    [Required]
    [StringLength(200, MinimumLength = 2)]
    public required string FullName { get; set; }

    public string Notes { get; set; } = string.Empty;

    [Required]
    public DateOnly? ExpiryDate { get; set; }

    [Range(0, int.MaxValue)]
    public int Quantity { get; set; }

    [Range(typeof(decimal), "0.01", "99999999.99")]
    public decimal Price { get; set; }

    [Required]
    [StringLength(100)]
    public required string Brand { get; set; }
}
