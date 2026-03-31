using System;
using System.Threading.Tasks;
using FlightTracker.Models;
using FlightTracker.Interfaces;
using System.IO;
using System.Text.Json;
namespace FlightTracker.Services;

public class ExportDataService : IExportDataService
{
    public async Task<FlightDataRoot> ExportDataAsync(FlightDataRoot data, string filePath)
    {
        return null;
    }
}