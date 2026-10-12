
using PharmacyApp.Api.Models;

namespace PharmacyApp.Api.Data;

public class PharmacyData
{
    public List<Medicine> Medicines { get; set; } = new();
    public List<Sale> Sales { get; set; } = new();
}
