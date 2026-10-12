
using System.Text.Json;

namespace PharmacyApp.Api.Data;

public class PharmacyJsonStore
{
    private readonly string _filePath;
    private readonly SemaphoreSlim _lock = new(1, 1);

    private readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    public PharmacyJsonStore(IWebHostEnvironment environment)
    {
        var directory = Path.Combine(
            environment.ContentRootPath, "Data");

        Directory.CreateDirectory(directory);

        _filePath = Path.Combine(directory, "pharmacy-data.json");
    }

    public async Task<T> ExecuteAsync<T>(
        Func<PharmacyData, Task<(T Result, bool Save)>> operation)
    {
        await _lock.WaitAsync();

        try
        {
            var data = await ReadAsync();
            var (result, save) = await operation(data);

            if (save)
                await WriteAsync(data);

            return result;
        }
        finally
        {
            _lock.Release();
        }
    }

    private async Task<PharmacyData> ReadAsync()
    {
        if (!File.Exists(_filePath))
            return new PharmacyData();

        await using var stream = File.OpenRead(_filePath);

        return await JsonSerializer.DeserializeAsync<PharmacyData>(
            stream, _options)
            ?? throw new InvalidDataException(
                "Pharmacy data is invalid.");
    }

    private async Task WriteAsync(PharmacyData data)
    {
        var tempPath = _filePath + "." +
            Guid.NewGuid().ToString("N") + ".tmp";

        try
        {
            await using (var stream = new FileStream(
                tempPath, FileMode.CreateNew, FileAccess.Write,
                FileShare.None))
            {
                await JsonSerializer.SerializeAsync(
                    stream, data, _options);

                await stream.FlushAsync();
            }

            File.Move(tempPath, _filePath, overwrite: true);
        }
        finally
        {
            if (File.Exists(tempPath))
                File.Delete(tempPath);
        }
    }
}
