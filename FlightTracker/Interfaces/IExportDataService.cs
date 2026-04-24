using System.Threading.Tasks;
using FlightTracker.Models;

namespace FlightTracker.Interfaces;

public interface IExportDataService
{
    Task ExportFlightsToCsvAsync(FlightDataRoot data, string filePath);

    Task ExportAnalyticsToCsvAsync(AnalyticsSummary analytics, string filePath);

    Task ExportAnalyticsToTextAsync(AnalyticsSummary analytics, string filePath);
}