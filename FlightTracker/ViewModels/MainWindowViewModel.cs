using FlightTracker.Interfaces;
using FlightTracker.Models;

namespace FlightTracker.ViewModels;

public partial class MainWindowViewModel(ILoadDataService _loadDataService, IAnalyticsService _analyticsService, IExportDataService _exportDataService): ViewModelBase
{
    private readonly ILoadDataService _loadDataService = _loadDataService;
    private readonly IAnalyticsService _analyticsService = _analyticsService;
    private readonly IExportDataService _exportDataService = _exportDataService;

    private FlightDataRoot _flightData = new FlightDataRoot();
    public FlightDataRoot FlightData
    {
        get => _flightData;
        set
        {
            if (SetProperty(ref _flightData, value))
            {
                RebuildAnalytics();
            }
        }
    }

    private AnalyticsSummary _analytics = new AnalyticsSummary();
    public AnalyticsSummary Analytics
    {
        get => _analytics;
        set => SetProperty(ref _analytics, value);
    }

    public void RebuildAnalytics(int topCount = 5)
    {
        Analytics = _analyticsService.BuildAnalytics(FlightData, topCount);
    }
}
