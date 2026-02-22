using System.Net.Http.Headers;
using Gizmo.Go.Core.Services;

namespace Gizmo.Go.UI.Providers
{
    /// <summary>
    /// HTTP message handler that attaches the Bearer token to outgoing API requests.
    /// </summary>
    public sealed class BearerTokenHandler : DelegatingHandler
    {
        private readonly IAuthService _authService;

        public BearerTokenHandler(IAuthService authService)
        {
            _authService = authService;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await _authService.GetTokenAsync(cancellationToken);

            if (!string.IsNullOrEmpty(token))
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
