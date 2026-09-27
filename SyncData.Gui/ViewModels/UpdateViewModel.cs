using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SyncData.Gui.Localization;
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

        // Re-localize dynamic texts when the language changes.
        Localizer.Instance.PropertyChanged += (_, _) => OnPropertyChanged(nameof(UpdateBannerText));
    }

    /// <summary>Updates can only be applied to a Velopack-installed build.</summary>
    public bool CanCheckForUpdates => _updateService.IsInstalled;

    public string UpdateBannerText =>
        string.IsNullOrEmpty(AvailableVersion)
            ? string.Empty
            : Localizer.Instance.Format("Update_Available", AvailableVersion);

    [RelayCommand]
    private async Task CheckForUpdatesAsync()
    {
        if (!_updateService.IsInstalled)
        {
            StatusMessage = Localizer.Instance["Update_NotInstalled"];
            return;
        }

        if (IsBusy)
        {
            return;
        }

        IsBusy = true;
        StatusMessage = Localizer.Instance["Update_Checking"];

        try
        {
            var available = await _updateService.CheckForUpdatesAsync();
            UpdateAvailable = available;
            AvailableVersion = _updateService.AvailableVersion;
            StatusMessage = available ? string.Empty : Localizer.Instance["Update_UpToDate"];
        }
        catch (Exception ex)
        {
            StatusMessage = Localizer.Instance.Format("Update_CheckFailed", ex.Message);
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
        StatusMessage = Localizer.Instance["Update_Downloading"];

        try
        {
            var progress = new Progress<int>(value => DownloadProgress = value);
            await _updateService.DownloadUpdatesAsync(progress);
            StatusMessage = Localizer.Instance["Update_Applying"];
            _updateService.ApplyUpdatesAndRestart();
        }
        catch (Exception ex)
        {
            StatusMessage = Localizer.Instance.Format("Update_Failed", ex.Message);
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

    partial void OnAvailableVersionChanged(string? value) => OnPropertyChanged(nameof(UpdateBannerText));
}
