using FlightTracker.Interfaces;
using FlightTracker.Models;
using System.Linq;
using System.Threading.Tasks;

namespace FlightTracker.ViewModels;

public partial class View3ViewModel : ViewModelBase
{
    private readonly ILoadDataService _loadDataService;
    private readonly IAnalyticsService _analyticsService;
    private readonly IExportDataService _exportDataService;

    public View3ViewModel(ILoadDataService loadDataService, IAnalyticsService analyticsService, IExportDataService exportDataService)
    {
        _loadDataService = loadDataService;
        _analyticsService = analyticsService;
        _exportDataService = exportDataService;
    }

    private AnalyticsSummary _analytics = new();
    public AnalyticsSummary Analytics
    {
        get => _analytics;
        set => SetProperty(ref _analytics, value);
    }

    private string _loadStatus = "Waiting for analytics...";
    public string LoadStatus
    {
        get => _loadStatus;
        set => SetProperty(ref _loadStatus, value);
    }

    private int _topRoutesCount;
    public int TopRoutesCount
    {
        get => _topRoutesCount;
        set => SetProperty(ref _topRoutesCount, value);
    }

    private int _topAirlinesCount;
    public int TopAirlinesCount
    {
        get => _topAirlinesCount;
        set => SetProperty(ref _topAirlinesCount, value);
    }

    private int _timeBucketsCount;
    public int TimeBucketsCount
    {
        get => _timeBucketsCount;
        set => SetProperty(ref _timeBucketsCount, value);
    }

    private string _topRoutePreview = "-";
    public string TopRoutePreview
    {
        get => _topRoutePreview;
        set => SetProperty(ref _topRoutePreview, value);
    }

    private string _topAirlinePreview = "-";
    public string TopAirlinePreview
    {
        get => _topAirlinePreview;
        set => SetProperty(ref _topAirlinePreview, value);
    }

    public async Task InitializeAsync(string filePath, int topCount = 5)
    {
        try
        {
            var data = await _loadDataService.LoadDataAsync(filePath);
            Analytics = _analyticsService.BuildAnalytics(data, topCount);

            TopRoutesCount = Analytics.TopRoutes.Count;
            TopAirlinesCount = Analytics.TopAirlines.Count;
            TimeBucketsCount = Analytics.TrafficByTimeOfDay.Count;

            var topRoute = Analytics.TopRoutes.FirstOrDefault();
            TopRoutePreview = topRoute is null
                ? "-"
                : $"{topRoute.DepartureAirport} -> {topRoute.ArrivalAirport} ({topRoute.FlightsCount})";

            var topAirline = Analytics.TopAirlines.FirstOrDefault();
            TopAirlinePreview = topAirline is null
                ? "-"
                : $"{topAirline.AirlineName} ({topAirline.FlightsCount})";

            LoadStatus = "Analytics loaded successfully";
        }
        catch (System.Exception ex)
        {
            LoadStatus = $"Analytics failed: {ex.Message}";
        }
    }

}
