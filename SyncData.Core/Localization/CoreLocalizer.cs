using System.Globalization;
using System.Resources;

namespace SyncData.Core.Localization;

/// <summary>
/// Resolves localized strings for the core library. The active culture can be
/// set explicitly by the host (the GUI) via <see cref="Culture"/>; otherwise the
/// current UI culture is used.
/// </summary>
public static class CoreLocalizer
{
    private static readonly ResourceManager Manager =
        new("SyncData.Core.Localization.Strings", typeof(CoreLocalizer).Assembly);

    /// <summary>Culture explicitly selected by the host, or null to follow the current UI culture.</summary>
    public static CultureInfo? Culture { get; set; }

    private static CultureInfo ActiveCulture => Culture ?? CultureInfo.CurrentUICulture;

    public static string Get(string key) => Manager.GetString(key, ActiveCulture) ?? key;

    public static string Format(string key, params object?[] args) =>
        string.Format(ActiveCulture, Get(key), args);
}
