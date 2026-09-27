using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SyncData.Gui.Services;

namespace SyncData.Gui.ViewModels;

/// <summary>
/// Handles update checking/downloading and exposes the state the view binds to.
/// </summary>
public partial class UpdateViewModel : ObservableObject
{
    private readonly IUpdateService _updateService;

    [ObservableProperty]
    private bool _updateAvailable;

    [ObservableProperty]
    private string? _availableVersion;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private int _downloadProgress;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private string _currentVersion = "0.0.0";

    public UpdateViewModel()
        : this(new VelopackUpdateService())
    {
    }

    public UpdateViewModel(IUpdateService updateService)
    {
        _updateService = updateService;
        CurrentVersion = _updateService.CurrentVersion;
    }

    /// <summary>Updates can only be applied to a Velopack-installed build.</summary>
    public bool CanCheckForUpdates => _updateService.IsInstalled;

    [RelayCommand]
    private async Task CheckForUpdatesAsync()
    {
        if (!_updateService.IsInstalled)
        {
            StatusMessage = "Las actualizaciones solo están disponibles en la versión instalada.";
            return;
        }

        if (IsBusy)
        {
            return;
        }

        IsBusy = true;
        StatusMessage = "Buscando actualizaciones...";

        try
        {
            var available = await _updateService.CheckForUpdatesAsync();
            UpdateAvailable = available;
            AvailableVersion = _updateService.AvailableVersion;
            StatusMessage = available
                ? $"Nueva versión disponible: {AvailableVersion}"
                : "Ya tienes la última versión.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"No se pudo comprobar actualizaciones: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Same as <see cref="CheckForUpdatesAsync"/> but only reports when a newer
    /// version is found (used for the silent startup check).
    /// </summary>
    public async Task CheckForUpdatesSilentlyAsync()
    {
        if (!_updateService.IsInstalled || IsBusy)
        {
            return;
        }

        try
        {
            var available = await _updateService.CheckForUpdatesAsync();
            UpdateAvailable = available;
            AvailableVersion = _updateService.AvailableVersion;
            if (available)
            {
                StatusMessage = $"Nueva versión disponible: {AvailableVersion}";
            }
        }
        catch
        {
            // Silent check: ignore network errors on startup.
        }
    }

    [RelayCommand(CanExecute = nameof(CanApplyUpdate))]
    private async Task ApplyUpdateAsync()
    {
        if (IsBusy)
        {
            return;
        }

        IsBusy = true;
        DownloadProgress = 0;
        StatusMessage = "Descargando actualización...";

        try
        {
            var progress = new Progress<int>(value => DownloadProgress = value);
            await _updateService.DownloadUpdatesAsync(progress);
            StatusMessage = "Aplicando actualización y reiniciando...";
            _updateService.ApplyUpdatesAndRestart();
        }
        catch (Exception ex)
        {
            StatusMessage = $"No se pudo actualizar: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private bool CanApplyUpdate() => UpdateAvailable && !IsBusy;

    [RelayCommand]
    private void DismissUpdate()
    {
        UpdateAvailable = false;
        StatusMessage = string.Empty;
    }

    partial void OnUpdateAvailableChanged(bool value) => ApplyUpdateCommand.NotifyCanExecuteChanged();

    partial void OnIsBusyChanged(bool value) => ApplyUpdateCommand.NotifyCanExecuteChanged();
}
