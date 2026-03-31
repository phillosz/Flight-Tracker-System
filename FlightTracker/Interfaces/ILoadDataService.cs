using System;
using System.Threading.Tasks;
using FlightTracker.Models;
namespace FlightTracker.Interfaces;

public interface ILoadDataService
{
    Task<FlightDataRoot> LoadDataAsync(string filePath);
}