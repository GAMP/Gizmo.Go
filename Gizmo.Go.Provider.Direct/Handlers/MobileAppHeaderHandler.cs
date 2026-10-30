using Gizmo.Web.Api.Models;

namespace Gizmo.Go.Provider.Direct.Handlers
{
    /// <summary>
    /// HTTP message handler that marks every outgoing API request as sent by the mobile app,
    /// so the server applies the mobile flags.
    /// </summary>
    public sealed class MobileAppHeaderHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Remove(UserApiHeaders.App);
            request.Headers.TryAddWithoutValidation(UserApiHeaders.App, UserApiHeaders.MobileApp);

            return base.SendAsync(request, cancellationToken);
        }
    }
}
