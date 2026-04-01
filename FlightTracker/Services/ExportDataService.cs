using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using FlightTracker.Interfaces;
using FlightTracker.Models;

namespace FlightTracker.Services;

public class ExportDataService : IExportDataService
{
    public async Task ExportFlightsToCsvAsync(FlightDataRoot data, string filePath)
    {
        ArgumentNullException.ThrowIfNull(data);
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var csv = new StringBuilder();
        csv.AppendLine("FlightNumber,AirlineName,AirlineCode,DepartureAirport,ArrivalAirport,ScheduledDeparture,ScheduledArrival,AircraftType,Status");

        foreach (var flight in data.Flights)
        {
            csv.AppendLine(string.Join(',',
                EscapeCsv(flight.FlightNumber),
                EscapeCsv(flight.AirlineName),
                EscapeCsv(flight.AirlineCode),
                EscapeCsv(flight.DepartureAirport),
                EscapeCsv(flight.ArrivalAirport),
                EscapeCsv(flight.ScheduledDeparture.ToString("yyyy-MM-dd HH:mm:ss")),
                EscapeCsv(flight.ScheduledArrival.ToString("yyyy-MM-dd HH:mm:ss")),
                EscapeCsv(flight.AircraftType),
                EscapeCsv(flight.Status)));
        }

        await File.WriteAllTextAsync(filePath, csv.ToString());
    }

    public async Task ExportAnalyticsToCsvAsync(AnalyticsSummary analytics, string filePath)
    {
        ArgumentNullException.ThrowIfNull(analytics);
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var csv = new StringBuilder();

        csv.AppendLine("TopRoutes");
        csv.AppendLine("DepartureAirport,ArrivalAirport,FlightsCount");
        foreach (var route in analytics.TopRoutes)
        {
            csv.AppendLine(string.Join(',',
                EscapeCsv(route.DepartureAirport),
                EscapeCsv(route.ArrivalAirport),
                route.FlightsCount));
        }

        csv.AppendLine();
        csv.AppendLine("TopAirlines");
        csv.AppendLine("AirlineName,FlightsCount");
        foreach (var airline in analytics.TopAirlines)
        {
            csv.AppendLine(string.Join(',',
                EscapeCsv(airline.AirlineName),
                airline.FlightsCount));
        }

        csv.AppendLine();
        csv.AppendLine("TrafficByTimeOfDay");
        csv.AppendLine("TimeOfDay,FlightsCount");
        foreach (var metric in analytics.TrafficByTimeOfDay)
        {
            csv.AppendLine(string.Join(',',
                EscapeCsv(metric.TimeOfDay),
                metric.FlightsCount));
        }

        await File.WriteAllTextAsync(filePath, csv.ToString());
    }

    private static string EscapeCsv(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        var escaped = value.Replace("\"", "\"\"");
        if (escaped.Contains(',') || escaped.Contains('"') || escaped.Contains('\n') || escaped.Contains('\r'))
        {
            return $"\"{escaped}\"";
        }

        return escaped;
    }
}