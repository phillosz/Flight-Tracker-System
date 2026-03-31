using System;
using System.Threading.Tasks;
using FlightTracker.Models;
namespace FlightTracker.Interfaces;

public interface IExportDataService
{
    Task<FlightDataRoot> ExportDataAsync(FlightDataRoot data, string filePath);
}