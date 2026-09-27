using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using SyncData.Gui.ViewModels;

namespace SyncData.Gui.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private async void OnBrowseSource(object? sender, RoutedEventArgs e)
    {
        var path = await PickFolderAsync("Selecciona la carpeta de origen");
        if (path is not null && DataContext is MainWindowViewModel viewModel)
        {
            viewModel.SourcePath = path;
        }
    }

    private async void OnBrowseTarget(object? sender, RoutedEventArgs e)
    {
        var path = await PickFolderAsync("Selecciona la carpeta de destino");
        if (path is not null && DataContext is MainWindowViewModel viewModel)
        {
            viewModel.TargetPath = path;
        }
    }

    private async Task<string?> PickFolderAsync(string title)
    {
        var folders = await StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = title,
            AllowMultiple = false
        });

        return folders.Count > 0 ? folders[0].TryGetLocalPath() : null;
    }
}
