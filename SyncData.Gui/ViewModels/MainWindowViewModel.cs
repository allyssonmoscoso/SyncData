using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SyncData.Configuration;
using SyncData.Core;
using SyncData.Gui.Services;

namespace SyncData.Gui.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly UiLogger _logger = new();
    private CancellationTokenSource? _cancellationTokenSource;

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
    private string _status = "Listo para sincronizar.";

    [ObservableProperty]
    private bool _isRunning;

    public ObservableCollection<string> LogEntries => _logger.Entries;

    public UpdateViewModel Update { get; } = new();

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
        Status = "Sincronizando...";
        _logger.Clear();
        _cancellationTokenSource = new CancellationTokenSource();

        var progress = new UiProgress(value => Progress = value * 100);

        try
        {
            var app = new SyncApplication(config, _logger, progress);
            var token = _cancellationTokenSource.Token;

            var success = await Task.Run(() => app.RunAsync(token), token);

            Status = success ? "Sincronización completada." : "Sincronización finalizada sin completar.";
        }
        catch (OperationCanceledException)
        {
            Status = "Sincronización cancelada.";
        }
        catch (Exception ex)
        {
            Status = $"Error: {ex.Message}";
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
        Status = "Cancelando...";
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
