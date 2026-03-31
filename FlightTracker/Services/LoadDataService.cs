using System;
using System.Threading.Tasks;
using FlightTracker.Models;
using FlightTracker.Interfaces;
using System.IO;
using System.Text.Json;
namespace FlightTracker.Services;

public class LoadDataService : ILoadDataService
{
    public async Task<FlightDataRoot> LoadDataAsync(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"The file '{filePath}' was not found.");
        }

        try
        {
            string jsonString = await File.ReadAllTextAsync(filePath);
            FlightDataRoot data = JsonSerializer.Deserialize<FlightDataRoot>(jsonString);
            return data ?? new FlightDataRoot();
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException("Failed to parse the JSON data.", ex);
        }
    }
}