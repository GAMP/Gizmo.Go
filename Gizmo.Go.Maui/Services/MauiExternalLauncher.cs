using Gizmo.Go.Core.Services;
using Microsoft.Extensions.Logging;

namespace Gizmo.Go.Maui.Services
{
    /// <summary>
    /// Opens an external URL using the appropriate MAUI API:
    /// - http/https → Browser.Default (native in-app browser: SafariViewController / Chrome Custom Tabs)
    /// - custom schemes (tg://, etc.) → Launcher.Default (OS delegates to the target app)
    /// </summary>
    public sealed class MauiExternalLauncher : IExternalLauncher
    {
        private readonly ILogger<MauiExternalLauncher> _logger;

        public MauiExternalLauncher(ILogger<MauiExternalLauncher> logger)
        {
            _logger = logger;
        }

        public async Task OpenAsync(string url, CancellationToken cancellationToken = default)
        {
            try
            {
                var uri = new Uri(url);

                if (uri.Scheme is "http" or "https")
                {
                    await Browser.Default.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
                }
                else
                {
                    await Launcher.Default.OpenAsync(uri);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to open external URL: {Url}", url);
            }
        }
    }
}
