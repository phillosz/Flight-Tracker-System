using System;
using System.IO;
using System.Text.Json;
using FlightTracker.Interfaces;
using FlightTracker.Models;

namespace FlightTracker.Services;

public class UserPreferencesService : IUserPreferencesService
{
    private readonly string _preferencesFilePath;

    public UserPreferencesService(string? storageDirectory = null)
    {
        var rootDirectory = storageDirectory ?? ResolveDataDirectory();

        _preferencesFilePath = Path.Combine(rootDirectory, "preferences.json");
    }

    private static string ResolveDataDirectory()
    {
        var candidates = new[]
        {
            Path.Combine(AppContext.BaseDirectory, "Data"),
            Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "Data")),
            Path.Combine(Directory.GetCurrentDirectory(), "FlightTracker", "Data"),
            Path.Combine(Directory.GetCurrentDirectory(), "Data")
        };

        foreach (var candidate in candidates)
        {
            if (Directory.Exists(candidate))
            {
                return candidate;
            }
        }

        return candidates[0];
    }

    public UserPreferences LoadPreferences()
    {
        if (!File.Exists(_preferencesFilePath))
        {
            return new UserPreferences();
        }

        try
        {
            var json = File.ReadAllText(_preferencesFilePath);
            return JsonSerializer.Deserialize<UserPreferences>(json) ?? new UserPreferences();
        }
        catch
        {
            return new UserPreferences();
        }
    }

    public void SavePreferences(UserPreferences preferences)
    {
        ArgumentNullException.ThrowIfNull(preferences);

        var directory = Path.GetDirectoryName(_preferencesFilePath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var json = JsonSerializer.Serialize(preferences, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        File.WriteAllText(_preferencesFilePath, json);
    }

    public void ClearPreferences()
    {
        if (File.Exists(_preferencesFilePath))
        {
            File.Delete(_preferencesFilePath);
        }
    }
}