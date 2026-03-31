using System;
using System.Collections.Generic;
using System.Linq;
using FlightTracker.Interfaces;
using FlightTracker.Models;

namespace FlightTracker.Services;

public class AnalyticsService : IAnalyticsService
{
    public AnalyticsSummary BuildAnalytics(FlightDataRoot data, int topCount = 5)
    {
        var flights = data?.Flights ?? [];

        var topRoutes = flights
            .GroupBy(f => new { f.DepartureAirport, f.ArrivalAirport })
            .Select(group => new RouteTrafficMetric
            {
                DepartureAirport = group.Key.DepartureAirport,
                ArrivalAirport = group.Key.ArrivalAirport,
                FlightsCount = group.Count()
            })
            .OrderByDescending(route => route.FlightsCount)
            .ThenBy(route => route.DepartureAirport)
            .ThenBy(route => route.ArrivalAirport)
            .Take(topCount)
            .ToList();

        var topAirlines = flights
            .GroupBy(f => f.AirlineName)
            .Select(group => new AirlineTrafficMetric
            {
                AirlineName = group.Key,
                FlightsCount = group.Count()
            })
            .OrderByDescending(airline => airline.FlightsCount)
            .ThenBy(airline => airline.AirlineName)
            .Take(topCount)
            .ToList();

        var trafficByTimeOfDay = flights
            .GroupBy(flight => GetTimeOfDayBucket(flight.ScheduledDeparture))
            .Select(group => new TimeOfDayTrafficMetric
            {
                TimeOfDay = group.Key,
                FlightsCount = group.Count()
            })
            .OrderBy(metric => GetTimeOfDayOrder(metric.TimeOfDay))
            .ToList();

        return new AnalyticsSummary
        {
            TopRoutes = topRoutes,
            TopAirlines = topAirlines,
            TrafficByTimeOfDay = trafficByTimeOfDay
        };
    }

    private static string GetTimeOfDayBucket(DateTime scheduledDeparture)
    {
        var hour = scheduledDeparture.Hour;

        if (hour >= 5 && hour < 12)
        {
            return "Morning";
        }

        if (hour >= 12 && hour < 17)
        {
            return "Afternoon";
        }

        if (hour >= 17 && hour < 22)
        {
            return "Evening";
        }

        return "Night";
    }

    private static int GetTimeOfDayOrder(string timeOfDay)
    {
        return timeOfDay switch
        {
            "Morning" => 0,
            "Afternoon" => 1,
            "Evening" => 2,
            _ => 3
        };
    }
}
