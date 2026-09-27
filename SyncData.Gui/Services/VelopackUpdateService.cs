using System;
using System.Threading;
using System.Threading.Tasks;
using Velopack;
using Velopack.Sources;

namespace SyncData.Gui.Services;

/// <summary>
/// Velopack-based <see cref="IUpdateService"/> that pulls updates from the
/// project's GitHub Releases.
/// </summary>
public sealed class VelopackUpdateService : IUpdateService
{
    private const string RepositoryUrl = "https://github.com/allyssonmoscoso/SyncData";

    private readonly UpdateManager? _manager;
    private UpdateInfo? _pendingUpdate;

    public VelopackUpdateService()
    {
        try
        {
            // prerelease: false -> only stable releases are offered.
            _manager = new UpdateManager(new GithubSource(RepositoryUrl, null, false));
        }
        catch
        {
            // Not installed / no locator available: updates are simply unavailable.
            _manager = null;
        }
    }

    public bool IsInstalled => _manager?.IsInstalled ?? false;

    public string CurrentVersion =>
        _manager?.CurrentVersion?.ToString()
        ?? typeof(VelopackUpdateService).Assembly.GetName().Version?.ToString(3)
        ?? "0.0.0";

    public string? AvailableVersion => _pendingUpdate?.TargetFullRelease?.Version?.ToString();

    public async Task<bool> CheckForUpdatesAsync(CancellationToken cancellationToken = default)
    {
        if (_manager is null || !_manager.IsInstalled)
        {
            return false;
        }

        _pendingUpdate = await _manager.CheckForUpdatesAsync();
        return _pendingUpdate is not null;
    }

    public async Task DownloadUpdatesAsync(IProgress<int>? progress = null, CancellationToken cancellationToken = default)
    {
        if (_manager is null || _pendingUpdate is null)
        {
            return;
        }

        await _manager.DownloadUpdatesAsync(
            _pendingUpdate,
            value => progress?.Report(value),
            cancellationToken);
    }

    public void ApplyUpdatesAndRestart()
    {
        if (_manager is null || _pendingUpdate is null)
        {
            return;
        }

        _manager.ApplyUpdatesAndRestart(_pendingUpdate.TargetFullRelease);
    }
}
