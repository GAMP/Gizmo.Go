using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Gizmo.Go.Core.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace Gizmo.Go.Web.Providers
{
    /// <summary>
    /// Bridges <see cref="IAuthService"/> to Blazor's <see cref="AuthenticationStateProvider"/>.
    /// Also implements <see cref="IAccessTokenProvider"/> for <see cref="AuthorizationMessageHandler"/> compatibility.
    /// </summary>
    public sealed class GoAuthenticationStateProvider : AuthenticationStateProvider, IAccessTokenProvider
    {
        private readonly IAuthService _authService;

        public GoAuthenticationStateProvider(IAuthService authService)
        {
            _authService = authService;
            _authService.StateChanged += OnAuthStateChanged;
        }

        private void OnAuthStateChanged(object? sender, AuthState state)
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _authService.GetTokenAsync();

            if (string.IsNullOrWhiteSpace(token))
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

            var claims = ParseClaimsFromJwt(token);
            var identity = new ClaimsIdentity(claims, "jwt");

            return new AuthenticationState(new ClaimsPrincipal(identity));
        }

        public ValueTask<AccessTokenResult> RequestAccessToken() => RequestAccessToken(new AccessTokenRequestOptions());

        public async ValueTask<AccessTokenResult> RequestAccessToken(AccessTokenRequestOptions options)
        {
            var token = await _authService.GetTokenAsync();

            if (string.IsNullOrWhiteSpace(token))
                return new AccessTokenResult(AccessTokenResultStatus.RequiresRedirect, null!, "/login", null);

            var accessToken = new AccessToken
            {
                Value = token
            };

            return new AccessTokenResult(AccessTokenResultStatus.Success, accessToken, null, null);
        }

        private static IEnumerable<Claim> ParseClaimsFromJwt(string token)
        {
            var handler = new JwtSecurityTokenHandler();

            if (!handler.CanReadToken(token))
                return [];

            var jwtToken = handler.ReadJwtToken(token);
            return jwtToken.Claims;
        }
    }
}
