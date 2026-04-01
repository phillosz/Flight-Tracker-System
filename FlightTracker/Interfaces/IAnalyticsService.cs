using FlightTracker.Models;

namespace FlightTracker.Interfaces;

public interface IAnalyticsService
{
    AnalyticsSummary BuildAnalytics(FlightDataRoot data, int topCount = 5);
}
