using System;
using Avalonia;
using Velopack;

namespace SyncData.Gui;

internal static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        // Must run first so Velopack can handle install/update hooks (no-op when
        // the app is not installed by Velopack).
        VelopackApp.Build().Run();

        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
