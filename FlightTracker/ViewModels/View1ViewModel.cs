using FlightTracker.Interfaces;
using FlightTracker.Models;
using System.Threading.Tasks;

namespace FlightTracker.ViewModels;

public partial class View1ViewModel : ViewModelBase
{
    private readonly ILoadDataService _loadDataService;
    private readonly IAnalyticsService _analyticsService;
    private readonly IExportDataService _exportDataService;

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

    public async Task InitializeAsync(string filePath)
    {
        FlightData = await _loadDataService.LoadDataAsync(filePath);
    }

}
