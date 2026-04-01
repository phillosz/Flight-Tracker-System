using FlightTracker.Interfaces;
using FlightTracker.Models;

namespace FlightTracker.ViewModels;

public partial class View1ViewModel(ILoadDataService _loadDataService, IAnalyticsService _analyticsService, IExportDataService _exportDataService): ViewModelBase
{
    private readonly ILoadDataService _loadDataService = _loadDataService;
    private readonly IAnalyticsService _analyticsService = _analyticsService;
    private readonly IExportDataService _exportDataService = _exportDataService;

}
