using System.Threading.Tasks;
using SyncData.Gui.ViewModels;
using SyncData.Test.TestDoubles;
using Xunit;

namespace SyncData.Test
{
    public class UpdateViewModelTests
    {
        [Fact]
        public void Constructor_ExposesCurrentVersion()
        {
            var viewModel = new UpdateViewModel(new FakeUpdateService { CurrentVersion = "2.3.4" });

            Assert.Equal("2.3.4", viewModel.CurrentVersion);
        }

        [Fact]
        public void CanCheckForUpdates_ReflectsInstalledState()
        {
            Assert.True(new UpdateViewModel(new FakeUpdateService { IsInstalled = true }).CanCheckForUpdates);
            Assert.False(new UpdateViewModel(new FakeUpdateService { IsInstalled = false }).CanCheckForUpdates);
        }

        [Fact]
        public async Task CheckForUpdates_WhenUpdateAvailable_ShowsBannerAndEnablesApply()
        {
            var service = new FakeUpdateService { IsInstalled = true, HasUpdate = true };
            var viewModel = new UpdateViewModel(service);

            await viewModel.CheckForUpdatesCommand.ExecuteAsync(null);

            Assert.True(viewModel.UpdateAvailable);
            Assert.Equal("1.0.1", viewModel.AvailableVersion);
            Assert.True(viewModel.ApplyUpdateCommand.CanExecute(null));
        }

        [Fact]
        public async Task CheckForUpdates_WhenUpToDate_HidesBanner()
        {
            var service = new FakeUpdateService { IsInstalled = true, HasUpdate = false };
            var viewModel = new UpdateViewModel(service);

            await viewModel.CheckForUpdatesCommand.ExecuteAsync(null);

            Assert.False(viewModel.UpdateAvailable);
            Assert.False(viewModel.ApplyUpdateCommand.CanExecute(null));
            Assert.Contains("última versión", viewModel.StatusMessage);
        }

        [Fact]
        public async Task CheckForUpdates_WhenNotInstalled_ShowsMessageAndDoesNotCallService()
        {
            var service = new FakeUpdateService { IsInstalled = false };
            var viewModel = new UpdateViewModel(service);

            await viewModel.CheckForUpdatesCommand.ExecuteAsync(null);

            Assert.False(service.CheckCalled);
            Assert.False(viewModel.UpdateAvailable);
            Assert.Contains("instalada", viewModel.StatusMessage);
        }

        [Fact]
        public async Task CheckForUpdates_WhenServiceFails_SetsErrorMessage()
        {
            var service = new FakeUpdateService { IsInstalled = true, ThrowOnCheck = true };
            var viewModel = new UpdateViewModel(service);

            await viewModel.CheckForUpdatesCommand.ExecuteAsync(null);

            Assert.False(viewModel.UpdateAvailable);
            Assert.Contains("No se pudo comprobar", viewModel.StatusMessage);
        }

        [Fact]
        public async Task ApplyUpdate_DownloadsAndApplies()
        {
            var service = new FakeUpdateService { IsInstalled = true, HasUpdate = true };
            var viewModel = new UpdateViewModel(service);
            await viewModel.CheckForUpdatesCommand.ExecuteAsync(null);

            await viewModel.ApplyUpdateCommand.ExecuteAsync(null);

            Assert.True(service.DownloadCalled);
            Assert.True(service.Applied);
        }

        [Fact]
        public async Task DismissUpdate_HidesBanner()
        {
            var service = new FakeUpdateService { IsInstalled = true, HasUpdate = true };
            var viewModel = new UpdateViewModel(service);
            await viewModel.CheckForUpdatesCommand.ExecuteAsync(null);
            Assert.True(viewModel.UpdateAvailable);

            viewModel.DismissUpdateCommand.Execute(null);

            Assert.False(viewModel.UpdateAvailable);
        }

        [Fact]
        public async Task CheckForUpdatesSilently_WhenUpdateAvailable_ShowsBanner()
        {
            var service = new FakeUpdateService { IsInstalled = true, HasUpdate = true };
            var viewModel = new UpdateViewModel(service);

            await viewModel.CheckForUpdatesSilentlyAsync();

            Assert.True(viewModel.UpdateAvailable);
            Assert.Contains("Nueva versión disponible", viewModel.StatusMessage);
        }

        [Fact]
        public async Task CheckForUpdatesSilently_WhenNotInstalled_DoesNothing()
        {
            var service = new FakeUpdateService { IsInstalled = false };
            var viewModel = new UpdateViewModel(service);

            await viewModel.CheckForUpdatesSilentlyAsync();

            Assert.False(service.CheckCalled);
            Assert.False(viewModel.UpdateAvailable);
        }
    }
}
