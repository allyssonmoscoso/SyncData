using System;
using System.Threading;
using System.Threading.Tasks;
using SyncData.Gui.Services;

namespace SyncData.Test.TestDoubles
{
    /// <summary>
    /// In-memory <see cref="IUpdateService"/> for testing the update view model.
    /// </summary>
    public sealed class FakeUpdateService : IUpdateService
    {
        public bool IsInstalled { get; set; } = true;

        public string CurrentVersion { get; set; } = "1.0.0";

        public string? AvailableVersion { get; private set; }

        public bool HasUpdate { get; set; }

        public bool ThrowOnCheck { get; set; }

        public bool CheckCalled { get; private set; }

        public bool DownloadCalled { get; private set; }

        public bool Applied { get; private set; }

        public Task<bool> CheckForUpdatesAsync(CancellationToken cancellationToken = default)
        {
            CheckCalled = true;

            if (ThrowOnCheck)
            {
                return Task.FromException<bool>(new InvalidOperationException("network down"));
            }

            AvailableVersion = HasUpdate ? "1.0.1" : null;
            return Task.FromResult(HasUpdate);
        }

        public Task DownloadUpdatesAsync(IProgress<int>? progress = null, CancellationToken cancellationToken = default)
        {
            DownloadCalled = true;
            progress?.Report(100);
            return Task.CompletedTask;
        }

        public void ApplyUpdatesAndRestart()
        {
            Applied = true;
        }
    }
}
