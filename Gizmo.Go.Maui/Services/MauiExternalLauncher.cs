using Gizmo.Go.Core.Services;
using Microsoft.Extensions.Logging;

namespace Gizmo.Go.Maui.Services
{
    /// <summary>
    /// Opens an external URL using the appropriate MAUI API:
    /// - https://t.me/ links are converted to tg:// deep links so the OS opens Telegram directly
    ///   via Launcher.Default, which gives a clean Window.Resumed signal when the user returns.
    /// - http/https (non-Telegram) → Browser.Default (SafariViewController / Chrome Custom Tabs)
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
                var targetUrl = TryConvertToTelegramDeepLink(url, out var deepLink) ? deepLink : url;
                var uri = new Uri(targetUrl);

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

        private static bool TryConvertToTelegramDeepLink(string url, out string result)
        {
            result = url;
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri)) return false;
            if (!uri.Host.Equals("t.me", StringComparison.OrdinalIgnoreCase)) return false;

            var domain = uri.AbsolutePath.TrimStart('/');
            if (string.IsNullOrEmpty(domain)) return false;

            var query = uri.Query.TrimStart('?');
            result = string.IsNullOrEmpty(query)
                ? $"tg://resolve?domain={domain}"
                : $"tg://resolve?domain={domain}&{query}";

            return true;
        }
    }
}
