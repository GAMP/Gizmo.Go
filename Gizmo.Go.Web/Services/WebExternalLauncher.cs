using Gizmo.Go.Core.Services;
using Microsoft.JSInterop;

namespace Gizmo.Go.Web.Services
{
    /// <summary>
    /// Opens an external URL via window.open.
    /// https://t.me/ links are converted to tg:// deep links so the browser shows the native
    /// "Open Telegram?" system dialog without navigating away from the current page,
    /// preserving the visibilitychange listener used for resume detection.
    /// Other URLs open in a new tab (_blank).
    /// </summary>
    public sealed class WebExternalLauncher : IExternalLauncher
    {
        private readonly IJSRuntime _jsRuntime;

        public WebExternalLauncher(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task OpenAsync(string url, CancellationToken cancellationToken = default)
        {
            var targetUrl = TryConvertToTelegramDeepLink(url, out var deepLink) ? deepLink : url;
            await _jsRuntime.InvokeVoidAsync("window.open", cancellationToken, targetUrl, "_blank");
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
