using System.Collections.Generic;

namespace FlightTracker.Models;

public class AnalyticsSummary
{
    public List<RouteTrafficMetric> TopRoutes { get; set; } = [];

    public List<AirlineTrafficMetric> TopAirlines { get; set; } = [];

    public List<TimeOfDayTrafficMetric> TrafficByTimeOfDay { get; set; } = [];
}

public class RouteTrafficMetric
{
    public string DepartureAirport { get; set; } = string.Empty;

    public string ArrivalAirport { get; set; } = string.Empty;

    public int FlightsCount { get; set; }
}

public class AirlineTrafficMetric
{
    public string AirlineName { get; set; } = string.Empty;

    public int FlightsCount { get; set; }
}

public class TimeOfDayTrafficMetric
{
    public string TimeOfDay { get; set; } = string.Empty;

    public int FlightsCount { get; set; }
}
