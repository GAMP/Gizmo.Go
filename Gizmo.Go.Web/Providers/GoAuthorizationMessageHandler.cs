using Gizmo.Go.Core.Configuration;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Options;

namespace Gizmo.Go.Web.Providers
{
    /// <summary>
    /// HTTP message handler that attaches the Bearer token to outgoing API requests.
    /// </summary>
    public sealed class GoAuthorizationMessageHandler : AuthorizationMessageHandler
    {
        public GoAuthorizationMessageHandler(
            IAccessTokenProvider accessTokenProvider,
            NavigationManager navigationManager,
            IOptions<GizmoGoSettings> options) : base(accessTokenProvider, navigationManager)
        {
            var hostUrl = string.IsNullOrWhiteSpace(options.Value.BackendUrl)
                ? navigationManager.BaseUri
                : options.Value.BackendUrl;

            var baseUri = new Uri(hostUrl);
            var uriBuilder = new UriBuilder()
            {
                Scheme = baseUri.Scheme,
                Host = baseUri.Host,
                Port = baseUri.Port
            };

            ConfigureHandler(new[] { uriBuilder.ToString() });
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                return await base.SendAsync(request, cancellationToken);
            }
            catch (AccessTokenNotAvailableException)
            {
                throw;
            }
        }
    }
}
