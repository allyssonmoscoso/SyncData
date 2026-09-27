using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SyncData.Configuration;
using SyncData.Core;
using SyncData.Gui.Localization;
using SyncData.Gui.Services;

namespace SyncData.Gui.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly UiLogger _logger = new();
    private readonly SettingsService _settingsService;
    private CancellationTokenSource? _cancellationTokenSource;
    private Language _selectedLanguage;

    [ObservableProperty]
    private string _sourcePath = string.Empty;

    [ObservableProperty]
    private string _targetPath = string.Empty;

    [ObservableProperty]
    private bool _verbose;

    [ObservableProperty]
    private bool _logToFile;

    [ObservableProperty]
    private bool _preserve;

    [ObservableProperty]
    private bool _excludeEnabled;

    [ObservableProperty]
    private string _excludePaths = string.Empty;

    [ObservableProperty]
    private double _progress;

    [ObservableProperty]
    private string _status = Localizer.Instance["Status_Ready"];

    [ObservableProperty]
    private bool _isRunning;

    public MainWindowViewModel()
        : this(new SettingsService())
    {
    }

    public MainWindowViewModel(SettingsService settingsService)
    {
        _settingsService = settingsService;

        var currentCode = Localizer.Instance.CurrentLanguageCode;
        _selectedLanguage = Languages.FirstOrDefault(l => l.Code == currentCode) ?? Languages[0];
    }

    public ObservableCollection<string> LogEntries => _logger.Entries;

    public UpdateViewModel Update { get; } = new();

    public IReadOnlyList<Language> Languages => Localizer.Instance.AvailableLanguages;

    public Language SelectedLanguage
    {
        get => _selectedLanguage;
        set
        {
            if (value is null || !SetProperty(ref _selectedLanguage, value))
            {
                return;
            }

            Localizer.Instance.SetLanguage(value.Code);
            _settingsService.Save(new AppSettings { Language = value.Code });

            if (!IsRunning)
            {
                Status = Localizer.Instance["Status_Ready"];
            }
        }
    }

    public bool IsNotRunning => !IsRunning;

    partial void OnIsRunningChanged(bool value)
    {
        OnPropertyChanged(nameof(IsNotRunning));
        StartSyncCommand.NotifyCanExecuteChanged();
        CancelSyncCommand.NotifyCanExecuteChanged();
    }

    private bool CanStart() => !IsRunning;

    [RelayCommand(CanExecute = nameof(CanStart))]
    private async Task StartSyncAsync()
    {
        var config = BuildConfiguration();

        IsRunning = true;
        Progress = 0;
        Status = Localizer.Instance["Status_Syncing"];
        _logger.Clear();
        _cancellationTokenSource = new CancellationTokenSource();

        var progress = new UiProgress(value => Progress = value * 100);

        try
        {
            var app = new SyncApplication(config, _logger, progress);
            var token = _cancellationTokenSource.Token;

            var success = await Task.Run(() => app.RunAsync(token), token);

            Status = success
                ? Localizer.Instance["Status_Completed"]
                : Localizer.Instance["Status_NotCompleted"];
        }
        catch (OperationCanceledException)
        {
            Status = Localizer.Instance["Status_Cancelled"];
        }
        catch (Exception ex)
        {
            Status = Localizer.Instance.Format("Status_Error", ex.Message);
        }
        finally
        {
            IsRunning = false;
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
        }
    }

    [RelayCommand(CanExecute = nameof(IsRunning))]
    private void CancelSync()
    {
        _cancellationTokenSource?.Cancel();
        Status = Localizer.Instance["Status_Cancelling"];
    }

    private SyncConfiguration BuildConfiguration()
    {
        var config = new SyncConfiguration
        {
            SourcePath = SourcePath.Trim(),
            TargetPath = TargetPath.Trim(),
            Verbose = Verbose,
            LogToFile = LogToFile,
            PreservePermissionsAndTimestamps = Preserve,
            Exclude = ExcludeEnabled
        };

        if (ExcludeEnabled && !string.IsNullOrWhiteSpace(ExcludePaths))
        {
            var separators = new[] { ',' };
            foreach (var path in ExcludePaths.Split(separators, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                config.ExcludePaths.Add(path);
            }
        }

        return config;
    }
}
