using CommunityToolkit.Mvvm.Input;
using FlightTracker.Interfaces;
using FlightTracker.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightTracker.ViewModels;

public partial class View1ViewModel : ViewModelBase
{
    private readonly ILoadDataService _loadDataService;
    private readonly IAnalyticsService _analyticsService;
    private readonly IExportDataService _exportDataService;

    public event System.Action? FlightPathsUpdated;

    private List<(double oLon, double oLat, double dLon, double dLat)> _flightPaths = [];
    public List<(double oLon, double oLat, double dLon, double dLat)> FlightPaths
    {
        get => _flightPaths;
        private set => SetProperty(ref _flightPaths, value);
    }

    public View1ViewModel(ILoadDataService loadDataService, IAnalyticsService analyticsService, IExportDataService exportDataService)
    {
        _loadDataService = loadDataService;
        _analyticsService = analyticsService;
        _exportDataService = exportDataService;
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
            }
        }
    }

    public async Task InitializeAsync(string filePath)
    {
        FlightData = await _loadDataService.LoadDataAsync(filePath);
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

}
