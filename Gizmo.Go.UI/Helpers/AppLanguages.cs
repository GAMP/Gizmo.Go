using System.Globalization;
using Microsoft.Extensions.Logging;

namespace Gizmo.Go.UI.Helpers;

/// <summary>
/// UI languages of the app: the resource languages of Gizmo.Go.UI (neutral English and the satellite assemblies).
/// </summary>
public static class AppLanguages
{
    /// <summary>One language of the language screen.</summary>
    /// <param name="Code">Culture name.</param>
    /// <param name="NativeName">Name in the language itself, first letter upper case.</param>
    /// <param name="EnglishName">English name.</param>
    public sealed record AppLanguage(string Code, string NativeName, string EnglishName);

    // must match the satellite resource files of Gizmo.Go.UI
    private static readonly string[] Codes = ["en", "ru", "ar", "da", "el", "es-BO", "fi", "pt-BR", "tr"];

    /// <summary>
    /// Gets the languages sorted by the native name in the current culture; a culture missing in the runtime is skipped (logged).
    /// </summary>
    public static IReadOnlyList<AppLanguage> GetAll(ILogger logger)
    {
        var languages = new List<AppLanguage>(Codes.Length);

        foreach (var code in Codes)
        {
            try
            {
                var culture = new CultureInfo(code);
                languages.Add(new AppLanguage(code, GetNativeName(culture), culture.EnglishName));
            }
            catch (CultureNotFoundException ex)
            {
                logger.LogWarning(ex, "Culture {Culture} is not available, the language is skipped.", code);
            }
        }

        var comparer = StringComparer.Create(CultureInfo.CurrentUICulture, ignoreCase: true);
        languages.Sort((a, b) => comparer.Compare(a.NativeName, b.NativeName));

        return languages;
    }

    /// <summary>
    /// Gets the code of the language that matches the current UI culture (exact name, then the parent, then the two-letter
    /// language name); null when none matches.
    /// </summary>
    public static string? FindCurrentCode(IReadOnlyList<AppLanguage> languages)
    {
        var current = CultureInfo.CurrentUICulture;

        return languages.FirstOrDefault(x => string.Equals(x.Code, current.Name, StringComparison.OrdinalIgnoreCase))?.Code
            ?? languages.FirstOrDefault(x => current.Parent.Name.Length > 0 && string.Equals(x.Code, current.Parent.Name, StringComparison.OrdinalIgnoreCase))?.Code
            ?? languages.FirstOrDefault(x => current.TwoLetterISOLanguageName.Length > 0
                && string.Equals(new CultureInfo(x.Code).TwoLetterISOLanguageName, current.TwoLetterISOLanguageName, StringComparison.OrdinalIgnoreCase))?.Code;
    }

    /// <summary>
    /// Gets the name of a culture in the language itself, first letter upper case.
    /// </summary>
    public static string GetNativeName(CultureInfo culture)
    {
        var name = culture.NativeName;
        return name.Length == 0 ? name : culture.TextInfo.ToUpper(name[0]) + name[1..];
    }
}
