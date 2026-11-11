using System.Globalization;
using Gizmo.Go.Core.Models;
using Gizmo.Go.Core.Services;
using Gizmo.UI;
using Microsoft.Extensions.Logging;

namespace Gizmo.Go.UI.Helpers;

public static class CultureHelper
{
    private static readonly TimeSpan ServerCultureTimeout = TimeSpan.FromSeconds(3);

    /// <summary>
    /// Sets the start UI culture: the stored device language, otherwise the server one (mobile options), otherwise the current
    /// culture stays. The server currency format is applied in every case it could be loaded. An unknown stored language is
    /// ignored; an unreachable server keeps the stored (or the current) culture without the server currency.
    /// </summary>
    /// <param name="mobileOptionsService">Server options.</param>
    /// <param name="storedLanguage">Culture name stored on the device; null when the user has not chosen a language.</param>
    /// <param name="logger">Logger.</param>
    public static async Task TrySetStartCultureAsync(IMobileOptionsService mobileOptionsService, string? storedLanguage, ILogger logger)
    {
        try
        {
            var options = await TryGetOptionsAsync(mobileOptionsService, logger);

            var culture = TryCreateCulture(storedLanguage, logger) ?? TryCreateCulture(options?.PreferredLanguage, logger);

            if (culture is not null)
            {
                CultureInfo.DefaultThreadCurrentCulture = culture;
                CultureInfo.DefaultThreadCurrentUICulture = culture;
            }

            if (options?.Currency is { } currency)
            {
                var formatCulture = (CultureInfo)(CultureInfo.DefaultThreadCurrentCulture ?? CultureInfo.CurrentCulture).Clone();
                ApplyCurrency(formatCulture.NumberFormat, currency);
                CultureInfo.DefaultThreadCurrentCulture = formatCulture;
                CultureInfo.DefaultThreadCurrentUICulture ??= CultureInfo.CurrentUICulture;
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Start culture not applied.");
        }
    }

    /// <summary>
    /// Creates the culture of the chosen language with the server currency format (the options are cached after the first
    /// success); null when the language is unknown. An unreachable server gives the culture without the server currency.
    /// </summary>
    /// <param name="mobileOptionsService">Server options.</param>
    /// <param name="language">Culture name.</param>
    /// <param name="logger">Logger.</param>
    public static async Task<CultureInfo?> CreateCultureAsync(IMobileOptionsService mobileOptionsService, string language, ILogger logger)
    {
        var culture = TryCreateCulture(language, logger);

        if (culture is null)
            return null;

        var options = await TryGetOptionsAsync(mobileOptionsService, logger);

        if (options?.Currency is not { } currency)
            return culture;

        var formatCulture = (CultureInfo)culture.Clone();
        ApplyCurrency(formatCulture.NumberFormat, currency);
        return formatCulture;
    }

    private static async Task<MobileOptions?> TryGetOptionsAsync(IMobileOptionsService mobileOptionsService, ILogger logger)
    {
        try
        {
            using var timeout = new CancellationTokenSource(ServerCultureTimeout);
            return await mobileOptionsService.GetAsync(timeout.Token);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Server options for the culture not loaded.");
            return null;
        }
    }

    private static CultureInfo? TryCreateCulture(string? name, ILogger logger)
    {
        if (string.IsNullOrWhiteSpace(name))
            return null;

        try
        {
            return new CultureInfo(name);
        }
        catch (CultureNotFoundException ex)
        {
            logger.LogWarning(ex, "Unknown culture {Culture} ignored.", name);
            return null;
        }
    }

    // as the client (ClientLocalizationServiceBase.ConfigureLocalizationOptions): a set server value replaces the culture one
    private static void ApplyCurrency(NumberFormatInfo format, CurrencyOptions currency)
    {
        if (!string.IsNullOrWhiteSpace(currency.CurrencySymbol))
            format.CurrencySymbol = currency.CurrencySymbol;

        if (currency.CurrencyDecimalDigits.HasValue)
            format.CurrencyDecimalDigits = currency.CurrencyDecimalDigits.Value;

        if (!string.IsNullOrWhiteSpace(currency.CurrencyDecimalSeparator))
            format.CurrencyDecimalSeparator = currency.CurrencyDecimalSeparator;

        if (!string.IsNullOrWhiteSpace(currency.CurrencyGroupSeparator))
            format.CurrencyGroupSeparator = currency.CurrencyGroupSeparator;
    }
}
