using System;
using System.ComponentModel;
using System.Globalization;
using SyncData.Core.Localization;
using SyncData.Gui.Localization;
using Xunit;

namespace SyncData.Test
{
    public class LocalizerTests : IDisposable
    {
        public LocalizerTests() => Localizer.Instance.SetLanguage("en");

        public void Dispose()
        {
            Localizer.Instance.SetLanguage("en");
            CoreLocalizer.Culture = null;
        }

        [Fact]
        public void Default_IsEnglish()
        {
            Assert.Equal("Source:", Localizer.Instance["Label_Source"]);
        }

        [Fact]
        public void AvailableLanguages_ContainsEnglishAndSpanish()
        {
            Assert.Contains(Localizer.Instance.AvailableLanguages, l => l.Code == "en");
            Assert.Contains(Localizer.Instance.AvailableLanguages, l => l.Code == "es");
        }

        [Fact]
        public void SetLanguage_Spanish_ReturnsSpanish()
        {
            Localizer.Instance.SetLanguage("es");

            Assert.Equal("Origen:", Localizer.Instance["Label_Source"]);
        }

        [Fact]
        public void SetLanguage_RaisesPropertyChanged()
        {
            var raised = false;
            PropertyChangedEventHandler handler = (_, _) => raised = true;
            Localizer.Instance.PropertyChanged += handler;

            try
            {
                Localizer.Instance.SetLanguage("es");
            }
            finally
            {
                Localizer.Instance.PropertyChanged -= handler;
            }

            Assert.True(raised);
        }

        [Fact]
        public void Format_ReplacesPlaceholder()
        {
            Assert.Equal("New version available: 9.9.9", Localizer.Instance.Format("Update_Available", "9.9.9"));
        }

        [Fact]
        public void UnknownKey_ReturnsKey()
        {
            Assert.Equal("Does_Not_Exist", Localizer.Instance["Does_Not_Exist"]);
        }

        [Fact]
        public void SetLanguage_InvalidCulture_FallsBackToEnglishText()
        {
            Localizer.Instance.SetLanguage("not-a-valid-culture");

            Assert.Equal("Source:", Localizer.Instance["Label_Source"]);
        }

        [Fact]
        public void SetLanguage_ConfiguresCoreCulture()
        {
            Localizer.Instance.SetLanguage("es");

            Assert.Equal("es", CoreLocalizer.Culture?.Name);
            Assert.Equal("es", CultureInfo.CurrentUICulture.Name);
        }
    }
}
