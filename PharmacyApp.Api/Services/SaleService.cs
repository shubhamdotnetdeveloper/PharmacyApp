
using PharmacyApp.Api.Data;
using PharmacyApp.Api.DTOs;
using PharmacyApp.Api.Models;

namespace PharmacyApp.Api.Services;

public class SaleService
{
    private readonly PharmacyJsonStore _store;

    public SaleService(PharmacyJsonStore store)
    {
        _store = store;
    }

    public Task<List<Sale>> GetAllAsync()
    {
        return _store.ExecuteAsync(data =>
        {
            var sales = data.Sales
                .OrderByDescending(s => s.SoldAt)
                .ToList();

            return Task.FromResult((sales, false));
        });
    }

    public Task<Sale> CreateAsync(CreateSaleRequest request)
    {
        if (request.MedicineId is null ||
            request.MedicineId == Guid.Empty)
        {
            throw new ArgumentException(
                "A valid medicine ID is required.");
        }

        if (request.QuantitySold <= 0)
            throw new ArgumentException(
                "Sale quantity must be greater than zero.");

        return _store.ExecuteAsync(data =>
        {
            var medicine = data.Medicines
                .FirstOrDefault(m => m.Id == request.MedicineId);

            if (medicine is null)
                throw new KeyNotFoundException(
                    "Medicine not found.");

            if (medicine.ExpiryDate < DateOnly.FromDateTime(
                    DateTime.Today))
            {
                throw new InvalidOperationException(
                    "Expired medicine cannot be sold.");
            }

            if (medicine.Quantity < request.QuantitySold)
            {
                throw new InvalidOperationException(
                    "Insufficient stock.");
            }

            var sale = new Sale
            {
                MedicineId = medicine.Id,
                MedicineName = medicine.FullName,
                QuantitySold = request.QuantitySold,
                UnitPrice = medicine.Price,
                TotalAmount = medicine.Price * request.QuantitySold,
                SoldAt = DateTimeOffset.UtcNow
            };

            medicine.Quantity -= request.QuantitySold;
            data.Sales.Add(sale);

            return Task.FromResult((sale, true));
        });
    }
}
