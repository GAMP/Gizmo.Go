using System.Globalization;

namespace Gizmo.Go.Web.Providers
{
    /// <summary>
    /// Delegating handler that adds the current UI culture to outgoing HTTP requests.
    /// </summary>
    public sealed class CultureDelegatingHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.Headers.AcceptLanguage.Count == 0)
                request.Headers.AcceptLanguage.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue(CultureInfo.CurrentUICulture.ToString()));

            return base.SendAsync(request, cancellationToken);
        }
    }
}
