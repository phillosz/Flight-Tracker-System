using FlightTracker.Interfaces;
using FlightTracker.Models;
using CommunityToolkit.Mvvm.Input;

namespace FlightTracker.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly ILoadDataService _loadDataService;
    private readonly IAnalyticsService _analyticsService;
    private readonly IExportDataService _exportDataService;

    private readonly View1ViewModel _view1ViewModel;
    private readonly View2ViewModel _view2ViewModel;
    private readonly View3ViewModel _view3ViewModel;

    public MainWindowViewModel(ILoadDataService loadDataService, IAnalyticsService analyticsService, IExportDataService exportDataService)
    {
        _loadDataService = loadDataService;
        _analyticsService = analyticsService;
        _exportDataService = exportDataService;

        _view1ViewModel = new View1ViewModel(_loadDataService, _analyticsService, _exportDataService);
        _view2ViewModel = new View2ViewModel(_loadDataService, _analyticsService, _exportDataService);
        _view3ViewModel = new View3ViewModel(_loadDataService, _analyticsService, _exportDataService);

        ShowView1Command = new RelayCommand(() => CurrentViewModel = _view1ViewModel);
        ShowView2Command = new RelayCommand(() => CurrentViewModel = _view2ViewModel);
        ShowView3Command = new RelayCommand(() => CurrentViewModel = _view3ViewModel);

        CurrentViewModel = _view1ViewModel;
    }

    private ViewModelBase _currentViewModel = null!;
    public ViewModelBase CurrentViewModel
    {
        get => _currentViewModel;
        set => SetProperty(ref _currentViewModel, value);
    }

    public IRelayCommand ShowView1Command { get; }
    public IRelayCommand ShowView2Command { get; }
    public IRelayCommand ShowView3Command { get; }

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
