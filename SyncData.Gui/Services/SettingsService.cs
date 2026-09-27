using System;
using System.IO;
using System.Text.Json;

namespace SyncData.Gui.Services;

/// <summary>User settings persisted between sessions.</summary>
public sealed class AppSettings
{
    public string Language { get; set; } = "en";
}

/// <summary>
/// Loads and saves user settings in the OS configuration folder
/// (~/.config, %APPDATA% or ~/Library/Application Support).
/// </summary>
public sealed class SettingsService
{
    private readonly string _directory;
    private readonly string _filePath;

    public SettingsService()
        : this(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SyncData"))
    {
    }

    public SettingsService(string directory)
    {
        _directory = directory;
        _filePath = Path.Combine(directory, "settings.json");
    }

    public AppSettings Load()
    {
        try
        {
            if (File.Exists(_filePath))
            {
                var json = File.ReadAllText(_filePath);
                return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
            }
        }
        catch
        {
            // Unreadable settings fall back to defaults.
        }

        return new AppSettings();
    }

    public void Save(AppSettings settings)
    {
        try
        {
            Directory.CreateDirectory(_directory);
            var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }
        catch
        {
            // Persisting settings is best-effort.
        }
    }
}
