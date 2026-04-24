using CommunityToolkit.Mvvm.Input;
using FlightTracker.Interfaces;
using FlightTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightTracker.ViewModels;

public partial class View1ViewModel : ViewModelBase
{
    private readonly ILoadDataService _loadDataService;
    private readonly IAnalyticsService _analyticsService;
    private readonly IExportDataService _exportDataService;
    private readonly IUserPreferencesService _userPreferencesService;

    private bool _isRestoringSelection;

    public event System.Action? FlightPathsUpdated;

    private List<(double oLon, double oLat, double dLon, double dLat)> _flightPaths = [];
    public List<(double oLon, double oLat, double dLon, double dLat)> FlightPaths
    {
        get => _flightPaths;
        private set => SetProperty(ref _flightPaths, value);
    }

    public View1ViewModel(ILoadDataService loadDataService, IAnalyticsService analyticsService, IExportDataService exportDataService, IUserPreferencesService userPreferencesService)
    {
        _loadDataService = loadDataService;
        _analyticsService = analyticsService;
        _exportDataService = exportDataService;
        _userPreferencesService = userPreferencesService;
    }

    private FlightDataRoot _flightData = new();
    public FlightDataRoot FlightData
    {
        get => _flightData;
        set => SetProperty(ref _flightData, value);
    }

    private Airport? _selectedAirport;
    public Airport? SelectedAirport
    {
        get => _selectedAirport;
        set
        {
            if (SetProperty(ref _selectedAirport, value))
            {
                ApplyFlightPathsToMap();
                PersistSelectedAirport();
            }
        }
    }

    private string _preferenceStatus = "No saved preferences yet.";
    public string PreferenceStatus
    {
        get => _preferenceStatus;
        set => SetProperty(ref _preferenceStatus, value);
    }

    private string _flightSearchQuery = string.Empty;
    public string FlightSearchQuery
    {
        get => _flightSearchQuery;
        set => SetProperty(ref _flightSearchQuery, value);
    }

    private string _searchStatus = "Showing all flights for selected airport.";
    public string SearchStatus
    {
        get => _searchStatus;
        set => SetProperty(ref _searchStatus, value);
    }

    public async Task InitializeAsync(string filePath)
    {
        FlightData = await _loadDataService.LoadDataAsync(filePath);
        RestoreSavedPreferences();
    }

    public Dictionary<string, List<(double oLon, double oLat, double dLon, double dLat)>> GetFlightPaths()
    {
        var paths = new Dictionary<string, List<(double oLon, double oLat, double dLon, double dLat)>>();

        foreach (var flight in FlightData.Flights)
        {
            var origin = GetLatLon(flight.DepartureAirport);
            var destination = GetLatLon(flight.ArrivalAirport);

            if (origin != null && destination != null)
            {
                if (!paths.ContainsKey(flight.DepartureAirport))
                {
                    paths[flight.DepartureAirport] = new List<(double oLon, double oLat, double dLon, double dLat)>();
                }

                paths[flight.DepartureAirport].Add((origin[1], origin[0], destination[1], destination[0]));
            }
        }

        return paths;
    }

    public double[] GetLatLon(string code)
    {
        return new double[] { 
        FlightData.Airports
        .Where(a => a.IataCode == code)
        .Select(a => a.Latitude)
        .FirstOrDefault(),
        FlightData.Airports
        .Where(a => a.IataCode == code)
        .Select(a => a.Longitude)
        .FirstOrDefault()
        };
    }

    [RelayCommand]
    private void SearchFlight()
    {
        var query = FlightSearchQuery?.Trim();
        if (string.IsNullOrWhiteSpace(query))
        {
            return;
        }

        var match = FlightData.Flights.FirstOrDefault(flight =>
            flight.FlightNumber.Equals(query, StringComparison.OrdinalIgnoreCase)
            || flight.AirlineName.Equals(query, StringComparison.OrdinalIgnoreCase));

        match ??= FlightData.Flights.FirstOrDefault(flight =>
            flight.FlightNumber.Contains(query, StringComparison.OrdinalIgnoreCase)
            || flight.AirlineName.Contains(query, StringComparison.OrdinalIgnoreCase));

        if (match is null)
        {
            return;
        }

        var airport = FlightData.Airports.FirstOrDefault(item =>
            item.IataCode.Equals(match.DepartureAirport, StringComparison.OrdinalIgnoreCase));

        if (airport is null)
        {
            return;
        }

        FlightSearchQuery = match.FlightNumber;
        SelectedAirport = airport;
        ApplyFlightPathsToMap();
        SearchStatus = $"Showing flight {match.FlightNumber}: {match.DepartureAirport} -> {match.ArrivalAirport}.";
    }

    [RelayCommand]
    private void ClearAll()
    {
        FlightSearchQuery = string.Empty;
        SelectedAirport = null;
        _userPreferencesService.ClearPreferences();
        PreferenceStatus = "Saved airport preference cleared.";
        SearchStatus = "Showing all flights for selected airport.";
    }

    [RelayCommand]
    public void ApplyFlightPathsToMap()
    {
        if (SelectedAirport is null)
        {
            FlightPaths = [];
            SearchStatus = "Select an airport to search routes.";
            FlightPathsUpdated?.Invoke();
            return;
        }

        var query = FlightSearchQuery?.Trim() ?? string.Empty;
        var selectedFlights = FlightData.Flights
            .Where(flight => flight.DepartureAirport == SelectedAirport.IataCode)
            .Where(flight => string.IsNullOrWhiteSpace(query)
                || flight.FlightNumber.Contains(query, StringComparison.OrdinalIgnoreCase)
                || flight.AirlineName.Contains(query, StringComparison.OrdinalIgnoreCase))
            .ToList();

        FlightPaths = selectedFlights
            .Select(flight =>
            {
                var origin = GetLatLon(flight.DepartureAirport);
                var destination = GetLatLon(flight.ArrivalAirport);
                return (origin, destination);
            })
            .Where(pair => pair.origin is not null && pair.destination is not null)
            .Select(pair => (pair.origin[1], pair.origin[0], pair.destination[1], pair.destination[0]))
            .ToList();

        SearchStatus = string.IsNullOrWhiteSpace(query)
            ? $"Showing {selectedFlights.Count} flights from {SelectedAirport.IataCode}."
            : $"Found {selectedFlights.Count} matching flights for '{query}'.";

        FlightPathsUpdated?.Invoke();
    }

    private void PersistSelectedAirport()
    {
        if (_isRestoringSelection)
        {
            return;
        }

        if (SelectedAirport is null)
        {
            _userPreferencesService.ClearPreferences();
            PreferenceStatus = "Saved airport preference cleared.";
            return;
        }

        _userPreferencesService.SavePreferences(new UserPreferences
        {
            LastSelectedAirportCode = SelectedAirport.IataCode
        });

        PreferenceStatus = $"Saved airport preference: {SelectedAirport.IataCode}";
    }

    private void RestoreSavedPreferences()
    {
        var preferences = _userPreferencesService.LoadPreferences();
        if (string.IsNullOrWhiteSpace(preferences.LastSelectedAirportCode))
        {
            PreferenceStatus = "No saved airport preference.";
            return;
        }

        var match = FlightData.Airports.FirstOrDefault(airport =>
            airport.IataCode.Equals(preferences.LastSelectedAirportCode, StringComparison.OrdinalIgnoreCase));

        if (match is null)
        {
            PreferenceStatus = $"Saved airport {preferences.LastSelectedAirportCode} was not found.";
            return;
        }

        _isRestoringSelection = true;
        SelectedAirport = match;
        _isRestoringSelection = false;

        PreferenceStatus = $"Restored saved airport: {match.IataCode}";
    }

}
