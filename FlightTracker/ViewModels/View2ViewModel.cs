using FlightTracker.Interfaces;
using FlightTracker.Models;
using System.Linq;
using System.Threading.Tasks;

namespace FlightTracker.ViewModels;

public partial class View2ViewModel : ViewModelBase
{
    private readonly ILoadDataService _loadDataService;
    private readonly IAnalyticsService _analyticsService;
    private readonly IExportDataService _exportDataService;

    public View2ViewModel(ILoadDataService loadDataService, IAnalyticsService analyticsService, IExportDataService exportDataService)
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

    private string _loadStatus = "Waiting for data...";
    public string LoadStatus
    {
        get => _loadStatus;
        set => SetProperty(ref _loadStatus, value);
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

    public async Task InitializeAsync(string filePath)
    {
        try
        {
            FlightData = await _loadDataService.LoadDataAsync(filePath);
            AirportCount = FlightData.Airports.Count;
            FlightCount = FlightData.Flights.Count;
            AirlineCount = FlightData.Flights.Select(f => f.AirlineCode).Distinct().Count();
            LoadStatus = "Data loaded successfully";
        }
        catch (System.Exception ex)
        {
            LoadStatus = $"Load failed: {ex.Message}";
        }
    }

    
}
