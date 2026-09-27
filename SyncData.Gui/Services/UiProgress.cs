using System;
using Avalonia.Threading;

namespace SyncData.Gui.Services;

/// <summary>
/// <see cref="IProgress{T}"/> implementation that marshals reports to the UI thread.
/// </summary>
public sealed class UiProgress : IProgress<double>
{
    private readonly Action<double> _onReport;

    public UiProgress(Action<double> onReport)
    {
        _onReport = onReport;
    }

    public void Report(double value)
    {
        Dispatcher.UIThread.Post(() => _onReport(value));
    }
}
