using System;
using FlightTracker.Models;
using FlightTracker.Services;

namespace FlightTracker.Tests;

public class AnalyticsServiceTests
{
    [Fact]
    public void GroupsAndSortsRoutesAndAirlines()
    {
        var service = new AnalyticsService();

        var data = new FlightDataRoot
        {
            Flights =
            [
                CreateFlight("ALPHA", "AAA", "BBB", 6),
                CreateFlight("ALPHA", "AAA", "BBB", 7),
                CreateFlight("BRAVO", "AAA", "CCC", 8),
                CreateFlight("BRAVO", "AAA", "CCC", 9),
                CreateFlight("CHARLIE", "BBB", "CCC", 13),
                CreateFlight("ALPHA", "CCC", "AAA", 18),
                CreateFlight("DELTA", "DDD", "AAA", 23)
            ]
        };

        var result = service.BuildAnalytics(data, topCount: 2);

        Assert.Equal(2, result.TopRoutes.Count);
        Assert.Equal("AAA", result.TopRoutes[0].DepartureAirport);
        Assert.Equal("BBB", result.TopRoutes[0].ArrivalAirport);

        Assert.Equal(2, result.TopAirlines.Count);
        Assert.Equal("ALPHA", result.TopAirlines[0].AirlineName);
    }

    [Fact]
    public void BucketsTimeOfDay_InExpectedOrder()
    {
        var service = new AnalyticsService();

        var data = new FlightDataRoot
        {
            Flights =
            [
                CreateFlight("A", "AAA", "BBB", 6),
                CreateFlight("B", "AAA", "BBB", 13),
                CreateFlight("C", "AAA", "BBB", 18),
                CreateFlight("D", "AAA", "BBB", 23),
                CreateFlight("E", "AAA", "BBB", 2)
            ]
        };

        var result = service.BuildAnalytics(data);

        Assert.Equal(4, result.TrafficByTimeOfDay.Count);

        Assert.Equal("Morning", result.TrafficByTimeOfDay[0].TimeOfDay);
        Assert.Equal(1, result.TrafficByTimeOfDay[0].FlightsCount);

        Assert.Equal("Night", result.TrafficByTimeOfDay[3].TimeOfDay);
        Assert.Equal(2, result.TrafficByTimeOfDay[3].FlightsCount);
    }

    private static Flight CreateFlight(string airline, string departure, string arrival, int departureHour)
    {
        return new Flight
        {
            FlightNumber = $"{airline}-{departure}-{arrival}-{departureHour}",
            AirlineName = airline,
            AirlineCode = airline.Substring(0, 1),
            DepartureAirport = departure,
            ArrivalAirport = arrival,
            ScheduledDeparture = new DateTime(2026, 4, 24, departureHour, 0, 0),
            ScheduledArrival = new DateTime(2026, 4, 24, departureHour, 30, 0),
            AircraftType = "A320",
            Status = "Scheduled"
        };
    }
}
