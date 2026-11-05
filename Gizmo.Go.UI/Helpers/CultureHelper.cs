using System.Globalization;
using Gizmo.Go.Core.Services;
using Gizmo.UI;
using Microsoft.Extensions.Logging;

namespace Gizmo.Go.UI.Helpers;

public static class CultureHelper
{
    private static readonly TimeSpan ServerCultureTimeout = TimeSpan.FromSeconds(3);

    /// <summary>
    /// Sets the server UI culture (mobile options) as the start culture with the server currency format; it is not persisted.
    /// An unreachable server or an unknown culture keeps the current culture.
    /// </summary>
    public static async Task TrySetServerCultureAsync(IMobileOptionsService mobileOptionsService, ILogger logger)
    {
        try
        {
            using var timeout = new CancellationTokenSource(ServerCultureTimeout);
            var options = await mobileOptionsService.GetAsync(timeout.Token);

            if (!string.IsNullOrWhiteSpace(options.PreferredLanguage))
            {
                var culture = new CultureInfo(options.PreferredLanguage);
                CultureInfo.DefaultThreadCurrentCulture = culture;
                CultureInfo.DefaultThreadCurrentUICulture = culture;
            }

            if (options.Currency is { } currency)
            {
                var culture = (CultureInfo)(CultureInfo.DefaultThreadCurrentCulture ?? CultureInfo.CurrentCulture).Clone();
                ApplyCurrency(culture.NumberFormat, currency);
                CultureInfo.DefaultThreadCurrentCulture = culture;
                CultureInfo.DefaultThreadCurrentUICulture ??= CultureInfo.CurrentUICulture;
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Server start culture not applied.");
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
