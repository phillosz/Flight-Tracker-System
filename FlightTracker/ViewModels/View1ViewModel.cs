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
    private void ClearSelection()
    {
        SelectedAirport = null;
        _userPreferencesService.ClearPreferences();
        PreferenceStatus = "Saved airport preference cleared.";
    }

    [RelayCommand]
    public void ApplyFlightPathsToMap()
    {
        if (SelectedAirport is null)
        {
            FlightPaths = [];
            FlightPathsUpdated?.Invoke();
            return;
        }

        var paths = GetFlightPaths();
        FlightPaths = paths.TryGetValue(SelectedAirport.IataCode, out var selectedPaths)
            ? selectedPaths
            : [];

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
