using CommunityToolkit.Mvvm.Input;
using FlightTracker.Interfaces;
using FlightTracker.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FlightTracker.ViewModels;

public partial class View2ViewModel : ViewModelBase
{
    private readonly ILoadDataService _loadDataService;
    private readonly IAnalyticsService _analyticsService;
    private readonly IExportDataService _exportDataService;
    private readonly IUserPreferencesService _userPreferencesService;

    public View2ViewModel(ILoadDataService loadDataService, IAnalyticsService analyticsService, IExportDataService exportDataService, IUserPreferencesService userPreferencesService)
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

    private string _loadStatus = "Waiting for data...";
    public string LoadStatus
    {
        get => _loadStatus;
        set => SetProperty(ref _loadStatus, value);
    }

    private string _exportStatus = "No export generated";
    public string ExportStatus
    {
        get => _exportStatus;
        set => SetProperty(ref _exportStatus, value);
    }

    private int _airportCount;
    public int AirportCount
    {
        get => _airportCount;
        set => SetProperty(ref _airportCount, value);
    }

    private int _flightCount;
    public int FlightCount
    {
        get => _flightCount;
        set => SetProperty(ref _flightCount, value);
    }

    private int _airlineCount;
    public int AirlineCount
    {
        get => _airlineCount;
        set => SetProperty(ref _airlineCount, value);
    }

    private List<Airport> _airports = [];
    public List<Airport> Airports
    {
        get => _airports;
        set => SetProperty(ref _airports, value);
    }

    private Airport? _selectedAirport;
    public Airport? SelectedAirport
    {
        get => _selectedAirport;
        set
        {
            if (SetProperty(ref _selectedAirport, value))
            {
                ShowSelectedAirportInfo();
                PersistSelectedAirport();
            }
        }
    }

    private string _selectedAirportInfo = "No airport selected";
    public string SelectedAirportInfo
    {
        get => _selectedAirportInfo;
        set => SetProperty(ref _selectedAirportInfo, value);
    }

    private List<Flight> _displayedFlights = [];
    public List<Flight> DisplayedFlights
    {
        get => _displayedFlights;
        set => SetProperty(ref _displayedFlights, value);
    }

    [RelayCommand]
    private void ShowSelectedAirportInfo()
    {
        if (SelectedAirport != null)
        {
            var flights = FlightData.Flights.Where(f => f.DepartureAirport == SelectedAirport.IataCode || f.ArrivalAirport == SelectedAirport.IataCode).ToList();
            SelectedAirportInfo = $"Flights from: {flights.Count(f => f.DepartureAirport == SelectedAirport.IataCode)}, Flights to: {flights.Count(f => f.ArrivalAirport == SelectedAirport.IataCode)}";
            StatusFilter = "All";
            ApplyFilters();
        }
        else
        {
            SelectedAirportInfo = "No airport selected";
            DisplayedFlights = [];
        }
    }

    public async Task InitializeAsync(string filePath)
    {
        try
        {
            FlightData = await _loadDataService.LoadDataAsync(filePath);
            Airports = FlightData.Airports;
            AirportCount = FlightData.Airports.Count;
            FlightCount = FlightData.Flights.Count;
            AirlineCount = FlightData.Flights.Select(f => f.AirlineCode).Distinct().Count();
            LoadStatus = "Data loaded successfully";
            RestoreSavedSelection();
        }
        catch (System.Exception ex)
        {
            LoadStatus = $"Load failed: {ex.Message}";
        }
    }

    private string _statusFilter = "All";
    public string StatusFilter
    {
        get => _statusFilter;
        set => SetProperty(ref _statusFilter, value);
    }

    [RelayCommand]
    private void FilterLanded()
    {
        StatusFilter = "Landed";
        ApplyFilters();
    }

    [RelayCommand]
    private void FilterScheduled()
    {
        StatusFilter = "Scheduled";
        ApplyFilters();
    }

    [RelayCommand]
    private void ResetFilters()
    {
        StatusFilter = "All";
        ApplyFilters();
    }

    private void ApplyFilters()
    {
        if (SelectedAirport == null)
        {
            DisplayedFlights = [];
            return;
        }

        var flights = FlightData.Flights
            .Where(f => f.DepartureAirport == SelectedAirport.IataCode || f.ArrivalAirport == SelectedAirport.IataCode)
            .ToList();

        if (StatusFilter == "Landed")
        {
            flights = flights.Where(f => f.Status.Equals("Landed", System.StringComparison.OrdinalIgnoreCase)).ToList();
        }
        else if (StatusFilter == "Scheduled")
        {
            flights = flights.Where(f => f.Status.Equals("Scheduled", System.StringComparison.OrdinalIgnoreCase)).ToList();
        }

        DisplayedFlights = flights;
    }

    private void PersistSelectedAirport()
    {
        if (SelectedAirport is null)
        {
            _userPreferencesService.ClearPreferences();
            return;
        }

        _userPreferencesService.SavePreferences(new UserPreferences
        {
            LastSelectedAirportCode = SelectedAirport.IataCode
        });
    }

    private void RestoreSavedSelection()
    {
        var preferences = _userPreferencesService.LoadPreferences();
        if (string.IsNullOrWhiteSpace(preferences.LastSelectedAirportCode))
        {
            return;
        }

        var airport = Airports.FirstOrDefault(item =>
            item.IataCode.Equals(preferences.LastSelectedAirportCode, StringComparison.OrdinalIgnoreCase));

        if (airport is not null)
        {
            SelectedAirport = airport;
        }
    }

    [RelayCommand]
    private async Task ExportFlightsCsvAsync()
    {
        try
        {
            var filteredFlights = DisplayedFlights.ToList();
            if (filteredFlights.Count == 0)
            {
                ExportStatus = "Export failed: no filtered flights to export.";
                return;
            }

            var exportDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Exports");
            var filePath = Path.Combine(exportDirectory, $"flights-{DateTime.Now:yyyyMMdd-HHmmss}.csv");
            var exportData = new FlightDataRoot
            {
                Flights = filteredFlights
            };

            await _exportDataService.ExportFlightsToCsvAsync(exportData, filePath);
            ExportStatus = $"Export created: {filePath}";
        }
        catch (Exception ex)
        {
            ExportStatus = $"Export failed: {ex.Message}";
        }
    }
}

