using System;
using System.IO;
using System.Linq;
using SyncData.Gui.Localization;
using SyncData.Gui.Services;
using SyncData.Gui.ViewModels;
using SyncData.Test.TestDoubles;
using Xunit;

namespace SyncData.Test
{
    public class MainWindowViewModelTests : IDisposable
    {
        public MainWindowViewModelTests() => Localizer.Instance.SetLanguage("en");

        public void Dispose() => Localizer.Instance.SetLanguage("en");

        [Fact]
        public void Constructor_DefaultsToEnglishAndReadyStatus()
        {
            using var dir = new TempDirectory();
            var viewModel = new MainWindowViewModel(new SettingsService(dir.Path));

            Assert.Equal("en", viewModel.SelectedLanguage.Code);
            Assert.Equal("Ready to synchronize.", viewModel.Status);
        }

        [Fact]
        public void SelectedLanguage_Change_UpdatesLocalizerAndStatus()
        {
            using var dir = new TempDirectory();
            var viewModel = new MainWindowViewModel(new SettingsService(dir.Path));

            viewModel.SelectedLanguage = viewModel.Languages.First(l => l.Code == "es");

            Assert.Equal("es", Localizer.Instance.CurrentLanguageCode);
            Assert.Equal("Origen:", Localizer.Instance["Label_Source"]);
            Assert.Equal("Listo para sincronizar.", viewModel.Status);
        }

        [Fact]
        public void SelectedLanguage_Change_PersistsSettings()
        {
            using var dir = new TempDirectory();
            var settingsService = new SettingsService(dir.Path);
            var viewModel = new MainWindowViewModel(settingsService);

            viewModel.SelectedLanguage = viewModel.Languages.First(l => l.Code == "es");

            Assert.True(File.Exists(Path.Combine(dir.Path, "settings.json")));
            Assert.Equal("es", settingsService.Load().Language);
        }

        [Fact]
        public void SettingsService_RoundTripsLanguage()
        {
            using var dir = new TempDirectory();
            var settingsService = new SettingsService(dir.Path);

            settingsService.Save(new AppSettings { Language = "es" });

            Assert.Equal("es", settingsService.Load().Language);
        }

        [Fact]
        public void SettingsService_WhenFileMissing_ReturnsDefaults()
        {
            using var dir = new TempDirectory();

            var settings = new SettingsService(dir.Path).Load();

            Assert.Equal("en", settings.Language);
        }
    }
}
