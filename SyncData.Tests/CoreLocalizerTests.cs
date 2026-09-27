using System;
using System.Globalization;
using SyncData.Core.Localization;
using Xunit;

namespace SyncData.Test
{
    public class CoreLocalizerTests : IDisposable
    {
        public void Dispose() => CoreLocalizer.Culture = null;

        [Fact]
        public void Get_EnglishCulture_ReturnsEnglish()
        {
            CoreLocalizer.Culture = CultureInfo.GetCultureInfo("en");

            Assert.Equal("The source path does not exist.", CoreLocalizer.Get("Validator_SourceMissing"));
        }

        [Fact]
        public void Get_SpanishCulture_ReturnsSpanish()
        {
            CoreLocalizer.Culture = CultureInfo.GetCultureInfo("es");

            Assert.Equal("La ruta de origen no existe.", CoreLocalizer.Get("Validator_SourceMissing"));
        }

        [Fact]
        public void Get_UnknownKey_ReturnsKey()
        {
            CoreLocalizer.Culture = CultureInfo.GetCultureInfo("en");

            Assert.Equal("Nope", CoreLocalizer.Get("Nope"));
        }

        [Fact]
        public void Format_ReplacesPlaceholder()
        {
            CoreLocalizer.Culture = CultureInfo.GetCultureInfo("en");

            Assert.Equal("Synchronization failed: boom", CoreLocalizer.Format("Sync_Failed", "boom"));
        }

        [Fact]
        public void Format_Spanish_ReplacesPlaceholder()
        {
            CoreLocalizer.Culture = CultureInfo.GetCultureInfo("es");

            Assert.Equal("La sincronización falló: boom", CoreLocalizer.Format("Sync_Failed", "boom"));
        }
    }
}
