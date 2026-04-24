using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FlightTracker.Interfaces;
using FlightTracker.Models;
using FlightTracker.ViewModels;

namespace FlightTracker.Tests;

public class View2ViewModelTests
{
    [Fact]
    public async Task InitializeAsync_PopulatesCountsAndAirports()
    {
        var flightData = new FlightDataRoot
        {
            Airports =
            [
                new Airport { IataCode = "AAA", Name = "Airport AAA" },
                new Airport { IataCode = "BBB", Name = "Airport BBB" }
            ],
            Flights =
            [
                CreateFlight("F1", "Alpha Air", "AA", "AAA", "BBB", "Landed"),
                CreateFlight("F2", "Beta Air", "BA", "BBB", "AAA", "Scheduled"),
                CreateFlight("F3", "Alpha Air", "AA", "AAA", "CCC", "Scheduled")
            ]
        };

        var viewModel = new View2ViewModel(
            new StubLoadDataService(flightData),
            new StubAnalyticsService(),
            new StubExportDataService(),
            new StubUserPreferencesService());

        await viewModel.InitializeAsync("unused-path");

        Assert.Equal("Data loaded successfully", viewModel.LoadStatus);
        Assert.Equal(2, viewModel.AirportCount);
        Assert.Equal(3, viewModel.FlightCount);
        Assert.Equal(2, viewModel.AirlineCount);
        Assert.Equal(2, viewModel.Airports.Count);
        Assert.Equal("Airport AAA", viewModel.Airports[0].Name);
    }

    [Fact]
    public void SelectingAirport_AndApplyingStatusFilter_UpdatesDisplayedFlights()
    {
        var viewModel = new View2ViewModel(
            new StubLoadDataService(new FlightDataRoot()),
            new StubAnalyticsService(),
            new StubExportDataService(),
            new StubUserPreferencesService())
        {
            FlightData = new FlightDataRoot
            {
                Flights =
                [
                    CreateFlight("F1", "Alpha Air", "AA", "AAA", "BBB", "Landed"),
                    CreateFlight("F2", "Beta Air", "BA", "BBB", "AAA", "Scheduled"),
                    CreateFlight("F3", "Alpha Air", "AA", "AAA", "CCC", "Scheduled"),
                    CreateFlight("F4", "Gamma Air", "GA", "DDD", "EEE", "Landed")
                ]
            }
        };

        viewModel.SelectedAirport = new Airport { IataCode = "AAA", Name = "Airport AAA" };

        Assert.Equal(3, viewModel.DisplayedFlights.Count);
        Assert.Contains(viewModel.DisplayedFlights, flight => flight.FlightNumber == "F1");
        Assert.Contains(viewModel.DisplayedFlights, flight => flight.FlightNumber == "F2");

        viewModel.FilterScheduledCommand.Execute(null);
        Assert.Equal("Scheduled", viewModel.StatusFilter);
        Assert.Equal(2, viewModel.DisplayedFlights.Count);

        viewModel.FilterLandedCommand.Execute(null);
        Assert.Equal("Landed", viewModel.StatusFilter);
        Assert.Single(viewModel.DisplayedFlights);
        Assert.Equal("F1", viewModel.DisplayedFlights[0].FlightNumber);

        viewModel.ResetFiltersCommand.Execute(null);
        Assert.Equal("All", viewModel.StatusFilter);
        Assert.Equal(3, viewModel.DisplayedFlights.Count);
    }

    private static Flight CreateFlight(string flightNumber, string airlineName, string airlineCode, string departureAirport, string arrivalAirport, string status)
    {
        return new Flight
        {
            FlightNumber = flightNumber,
            AirlineName = airlineName,
            AirlineCode = airlineCode,
            DepartureAirport = departureAirport,
            ArrivalAirport = arrivalAirport,
            ScheduledDeparture = new DateTime(2026, 4, 24, 10, 0, 0),
            ScheduledArrival = new DateTime(2026, 4, 24, 12, 0, 0),
            AircraftType = "A320",
            Status = status
        };
    }

    private sealed class StubLoadDataService : ILoadDataService
    {
        private readonly FlightDataRoot _data;

        public StubLoadDataService(FlightDataRoot data)
        {
            _data = data;
        }

        public Task<FlightDataRoot> LoadDataAsync(string filePath)
        {
            return Task.FromResult(_data);
        }
    }

    private sealed class StubAnalyticsService : IAnalyticsService
    {
        public AnalyticsSummary BuildAnalytics(FlightDataRoot data, int topCount = 5)
        {
            return new AnalyticsSummary();
        }
    }

    private sealed class StubExportDataService : IExportDataService
    {
        public Task ExportFlightsToCsvAsync(FlightDataRoot data, string filePath)
        {
            return Task.CompletedTask;
        }

        public Task ExportAnalyticsToCsvAsync(AnalyticsSummary analytics, string filePath)
        {
            return Task.CompletedTask;
        }

        public Task ExportAnalyticsToTextAsync(AnalyticsSummary analytics, string filePath)
        {
            return Task.CompletedTask;
        }
    }

    private sealed class StubUserPreferencesService : IUserPreferencesService
    {
        private UserPreferences _preferences = new();

        public UserPreferences LoadPreferences()
        {
            return _preferences;
        }

        public void SavePreferences(UserPreferences preferences)
        {
            _preferences = preferences;
        }

        public void ClearPreferences()
        {
            _preferences = new UserPreferences();
        }
    }
}
