using System.Globalization;
using Gizmo.Go.Core.Services;
using Microsoft.Extensions.Logging;

namespace Gizmo.Go.UI.Helpers;

public static class CultureHelper
{
    private static readonly TimeSpan ServerCultureTimeout = TimeSpan.FromSeconds(3);

    /// <summary>
    /// Sets the server UI culture (mobile options) as the start culture; it is not persisted.
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
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Server start culture not applied.");
        }
    }
}
