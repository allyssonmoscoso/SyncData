using System;
using System.Threading;
using System.Threading.Tasks;

namespace SyncData.Gui.Services;

/// <summary>
/// Abstraction over the update mechanism so the UI logic can be tested without
/// depending on Velopack or a real installation.
/// </summary>
public interface IUpdateService
{
    /// <summary>True when the app was installed by Velopack (updates only apply then).</summary>
    bool IsInstalled { get; }

    /// <summary>Currently installed version, e.g. "1.0.0".</summary>
    string CurrentVersion { get; }

    /// <summary>Version available after a successful check, or null.</summary>
    string? AvailableVersion { get; }

    /// <summary>Checks the update feed. Returns true when a newer version is available.</summary>
    Task<bool> CheckForUpdatesAsync(CancellationToken cancellationToken = default);

    /// <summary>Downloads the pending update, reporting 0..100 progress.</summary>
    Task DownloadUpdatesAsync(IProgress<int>? progress = null, CancellationToken cancellationToken = default);

    /// <summary>Applies the pending update and restarts the application.</summary>
    void ApplyUpdatesAndRestart();
}
