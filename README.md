# Flight Tracker System

Desktop flight-tracking app built with Avalonia and .NET 9.

## Project Structure Diagram

	Flight-Tracker-System/
	├── FlightTracker.sln
	├── FlightTracker/
	│   ├── Data/               # Flight and airport input data
	│   ├── Models/             # Domain models (Flight, Airport, Analytics, Preferences)
	│   ├── Interfaces/         # Service contracts
	│   ├── Services/           # Data load, analytics, export, user preferences
	│   ├── ViewModels/         # UI logic for View1, View2, View3, MainWindow
	│   ├── Views/              # Avalonia views
	│   └── Program.cs / App.axaml.cs
	└── FlightTracker.Tests/    # Unit tests

## Setup and Run

1. Install .NET SDK 9.0 or newer.
2. From the repository root, restore and build:

	dotnet restore
	dotnet build FlightTracker.sln

3. Run the app:

	dotnet run --project FlightTracker/FlightTracker.csproj

4. Run tests:

	dotnet test FlightTracker.sln

## App Components

- View 1 (Route Map): Select an airport and visualize routes on a map.
- View 2 (Flights): Browse flights by airport, filter by status, export filtered flights to CSV.
- View 3 (Analytics): Show top routes, top airlines, time-of-day traffic, export analytics report.
- AnalyticsService: Computes grouped and sorted metrics from flight data.
- ExportDataService: Writes flight exports (CSV) and analytics exports (CSV/TXT).
- UserPreferencesService: Persists last selected airport between sessions.

## Notes

- Input data files: FlightTracker/Data
- Export output folder: FlightTracker/Exports
