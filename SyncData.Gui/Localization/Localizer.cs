using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Resources;
using SyncData.Core.Localization;

namespace SyncData.Gui.Localization;

/// <summary>A selectable language.</summary>
public sealed record Language(string Code, string DisplayName);

/// <summary>
/// Application-wide localizer for the GUI. Exposes an indexer for XAML bindings
/// and raises change notifications so the UI updates live when the language
/// changes. It also configures the .NET cultures so the core library resolves
/// messages in the same language (including on background threads).
/// </summary>
public sealed class Localizer : INotifyPropertyChanged
{
    private static readonly ResourceManager Manager =
        new("SyncData.Gui.Localization.Strings", typeof(Localizer).Assembly);

    public static Localizer Instance { get; } = new();

    private CultureInfo _culture = CultureInfo.GetCultureInfo("en");

    private Localizer()
    {
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public IReadOnlyList<Language> AvailableLanguages { get; } = new[]
    {
        new Language("en", "English"),
        new Language("es", "Español"),
    };

    public string CurrentLanguageCode => _culture.Name;

    public void SetLanguage(string code)
    {
        CultureInfo culture;
        try
        {
            culture = CultureInfo.GetCultureInfo(code);
        }
        catch (Exception ex) when (ex is CultureNotFoundException or ArgumentException)
        {
            culture = CultureInfo.GetCultureInfo("en");
        }

        _culture = culture;

        CultureInfo.CurrentUICulture = culture;
        CultureInfo.CurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CoreLocalizer.Culture = culture;

        // Refresh every binding that uses the indexer.
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Item[]"));
    }

    public string this[string key] => Manager.GetString(key, _culture) ?? key;

    public string Format(string key, params object?[] args) =>
        string.Format(_culture, this[key], args);
}
