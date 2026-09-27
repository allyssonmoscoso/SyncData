using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using SyncData.Gui.ViewModels;
using SyncData.Gui.Views;

namespace SyncData.Gui;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var viewModel = new MainWindowViewModel();
            desktop.MainWindow = new MainWindow
            {
                DataContext = viewModel
            };

            // Check for updates in the background on startup (only when installed).
            if (viewModel.Update.CanCheckForUpdates)
            {
                _ = viewModel.Update.CheckForUpdatesSilentlyAsync();
            }
        }

        base.OnFrameworkInitializationCompleted();
    }
}
