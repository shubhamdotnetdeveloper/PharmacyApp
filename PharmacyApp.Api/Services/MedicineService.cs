
using PharmacyApp.Api.Data;
using PharmacyApp.Api.DTOs;
using PharmacyApp.Api.Models;

namespace PharmacyApp.Api.Services;

public class MedicineService
{
    private readonly PharmacyJsonStore _store;

    public MedicineService(PharmacyJsonStore store)
    {
        _store = store;
    }

    public Task<List<Medicine>> GetAllAsync(string? name)
    {
        return _store.ExecuteAsync(data =>
        {
            var medicines = data.Medicines
                .Where(m => string.IsNullOrWhiteSpace(name) ||
                    m.FullName.Contains(
                        name.Trim(),
                        StringComparison.OrdinalIgnoreCase))
                .OrderBy(m => m.FullName)
                .ToList();

            return Task.FromResult((medicines, false));
        });
    }

    public Task<Medicine?> GetByIdAsync(Guid id)
    {
        return _store.ExecuteAsync(data =>
        {
            var medicine = data.Medicines
                .FirstOrDefault(m => m.Id == id);

            return Task.FromResult((medicine, false));
        });
    }

    public Task<Medicine> AddAsync(CreateMedicineRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.FullName) ||
            string.IsNullOrWhiteSpace(request.Brand))
        {
            throw new ArgumentException(
                "Medicine name and brand are required.");
        }

        if (request.ExpiryDate is null)
            throw new ArgumentException("Expiry date is required.");

        if (request.Quantity < 0)
            throw new ArgumentException("Quantity cannot be negative.");

        if (request.Price <= 0 ||
            decimal.Round(request.Price, 2) != request.Price)
        {
            throw new ArgumentException(
                "Price must be positive with at most 2 decimal places.");
        }

        var medicine = new Medicine
        {
            FullName = request.FullName.Trim(),
            Notes = request.Notes?.Trim() ?? string.Empty,
            Brand = request.Brand.Trim(),
            ExpiryDate = request.ExpiryDate.Value,
            Quantity = request.Quantity,
            Price = request.Price
        };

        return _store.ExecuteAsync(data =>
        {
            data.Medicines.Add(medicine);
            return Task.FromResult((medicine, true));
        });
    }
}
