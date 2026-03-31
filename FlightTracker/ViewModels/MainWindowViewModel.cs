using FlightTracker.Interfaces;
using FlightTracker.Models;
using System;
using System.Threading.Tasks;

namespace FlightTracker.ViewModels;

public partial class MainWindowViewModel(ILoadDataService _loadDataService): ViewModelBase
{
    private readonly ILoadDataService _loadDataService = _loadDataService;

    private FlightDataRoot _flightData = new FlightDataRoot();
    public FlightDataRoot FlightData
    {
        get => _flightData;
        set => SetProperty(ref _flightData, value);
    }
}
