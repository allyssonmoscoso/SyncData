using System;
using System.Collections.ObjectModel;
using Avalonia.Threading;
using SyncData.Logging;

namespace SyncData.Gui.Services;

/// <summary>
/// <see cref="Logger"/> implementation that pushes log lines to the UI thread.
/// </summary>
public sealed class UiLogger : Logger
{
    public ObservableCollection<string> Entries { get; } = new();

    public override void Log(string status, string message)
    {
        var line = $"{DateTime.Now:HH:mm:ss} [{status}] {message}";

        if (Dispatcher.UIThread.CheckAccess())
        {
            Entries.Add(line);
        }
        else
        {
            Dispatcher.UIThread.Post(() => Entries.Add(line));
        }
    }

    public void Clear()
    {
        if (Dispatcher.UIThread.CheckAccess())
        {
            Entries.Clear();
        }
        else
        {
            Dispatcher.UIThread.Post(Entries.Clear);
        }
    }
}
