using FlightTracker.Models;

namespace FlightTracker.Interfaces;

public interface IUserPreferencesService
{
    UserPreferences LoadPreferences();

    void SavePreferences(UserPreferences preferences);

    void ClearPreferences();
}