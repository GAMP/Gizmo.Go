using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using Gizmo.Go.Core.Models;
using Gizmo.Go.Core.Services;
using Gizmo.Web.Api.Clients;
using Gizmo.Web.Api.Models;
using Microsoft.Extensions.DependencyInjection;
using UserAuthClient = Gizmo.Web.Api.User.Clients.AuthWebApiClient;

namespace Gizmo.Go.Provider.Direct.Services
{
    /// <summary>
    /// Direct Gizmo Server authentication service.
    /// </summary>
    internal sealed class DirectAuthService : IAuthService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ITokenStorageService _tokenStorageService;
        private readonly SemaphoreSlim _tokenLock = new(1, 1);
        private readonly JwtSecurityTokenHandler _jwtHandler = new();

        private string? _accessToken;
        private string? _refreshToken;
        private DateTime? _expiresUtc;

        public DirectAuthService(IServiceProvider serviceProvider, ITokenStorageService tokenStorageService)
        {
            _serviceProvider = serviceProvider;
            _tokenStorageService = tokenStorageService;
        }

        public AuthState State { get; private set; } = AuthState.Unknown;

        public UserProfile? CurrentUser { get; private set; }

        public event EventHandler<AuthState>? StateChanged;

        public async Task<AuthResult> LoginAsync(string username, string password, CancellationToken cancellationToken = default)
        {
            SetState(AuthState.Authenticating);

            try
            {
                var authClient = _serviceProvider.GetRequiredService<UserAuthClient>();

                var result = await authClient.AccessTokenGetAsync(new UserAccessTokenRequestModel
                {
                    Username = username,
                    Password = password
                }, cancellationToken);

                await _tokenLock.WaitAsync(cancellationToken);
                try
                {
                    _accessToken = result.Token;
                    _refreshToken = result.RefreshToken;
                    _expiresUtc = ReadExpiration(result.Token);
                    CurrentUser = ReadUserProfile(result.Token);
                }
                finally
                {
                    _tokenLock.Release();
                }

                await PersistTokensAsync(cancellationToken);

                SetState(AuthState.Authenticated);
                return AuthResult.Succeeded();
            }
            catch (WebApiClientException ex) when (ex.IsHttpStatusCode(HttpStatusCode.Unauthorized))
            {
                SetState(AuthState.Unauthenticated);
                return AuthResult.Failed(AuthErrorCode.InvalidCredentials);
            }
            catch (HttpRequestException)
            {
                SetState(AuthState.Unauthenticated);
                return AuthResult.Failed(AuthErrorCode.ConnectionFailed);
            }
            catch
            {
                SetState(AuthState.Unauthenticated);
                return AuthResult.Failed(AuthErrorCode.Unexpected);
            }
        }

        public async Task LogoutAsync(CancellationToken cancellationToken = default)
        {
            await _tokenStorageService.ClearAsync(cancellationToken);

            _accessToken = null;
            _refreshToken = null;
            _expiresUtc = null;
            CurrentUser = null;

            SetState(AuthState.Unauthenticated);
        }

        public async Task<string?> GetTokenAsync(CancellationToken cancellationToken = default)
        {
            await _tokenLock.WaitAsync(cancellationToken);
            try
            {
                if (string.IsNullOrWhiteSpace(_accessToken))
                    return null;

                // refresh if expired
                if (_expiresUtc.HasValue && _expiresUtc.Value <= DateTime.UtcNow)
                {
                    await RefreshTokenAsync(cancellationToken);
                }

                return _accessToken;
            }
            finally
            {
                _tokenLock.Release();
            }
        }

        public async Task TryRestoreSessionAsync(CancellationToken cancellationToken = default)
        {
            var stored = await _tokenStorageService.GetAsync(cancellationToken);

            if (stored == null || string.IsNullOrWhiteSpace(stored.Token))
            {
                SetState(AuthState.Unauthenticated);
                return;
            }

            await _tokenLock.WaitAsync(cancellationToken);
            try
            {
                _accessToken = stored.Token;
                _refreshToken = stored.RefreshToken;
                _expiresUtc = stored.ExpiresUtc ?? ReadExpiration(stored.Token);
                CurrentUser = ReadUserProfile(stored.Token);
            }
            finally
            {
                _tokenLock.Release();
            }

            // if the access token is expired, attempt a refresh
            if (_expiresUtc.HasValue && _expiresUtc.Value <= DateTime.UtcNow)
            {
                await _tokenLock.WaitAsync(cancellationToken);
                try
                {
                    await RefreshTokenAsync(cancellationToken);
                }
                finally
                {
                    _tokenLock.Release();
                }

                // RefreshTokenAsync calls LogoutAsync on failure, which clears everything
                if (State == AuthState.Unauthenticated)
                    return;
            }

            SetState(AuthState.Authenticated);
        }

        private async Task RefreshTokenAsync(CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(_accessToken) || string.IsNullOrWhiteSpace(_refreshToken))
            {
                await LogoutAsync(cancellationToken);
                return;
            }

            try
            {
                var authClient = _serviceProvider.GetRequiredService<UserAuthClient>();

                var result = await authClient.AccessTokenRefreshAsync(new UserAccessTokenRefreshRequestModel
                {
                    Token = _accessToken,
                    RefreshToken = _refreshToken
                }, cancellationToken);

                _accessToken = result.Token;
                _refreshToken = result.RefreshToken;
                _expiresUtc = ReadExpiration(result.Token);
                CurrentUser = ReadUserProfile(result.Token);

                await PersistTokensAsync(cancellationToken);
            }
            catch
            {
                await LogoutAsync(cancellationToken);
            }
        }

        private async Task PersistTokensAsync(CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(_accessToken) || string.IsNullOrWhiteSpace(_refreshToken))
                return;

            await _tokenStorageService.SetAsync(new AuthToken
            {
                Token = _accessToken,
                RefreshToken = _refreshToken,
                ExpiresUtc = _expiresUtc
            }, cancellationToken);
        }

        private DateTime? ReadExpiration(string token)
        {
            if (!_jwtHandler.CanReadToken(token))
                return null;

            var jwt = _jwtHandler.ReadJwtToken(token);
            return jwt.ValidTo == DateTime.MinValue ? null : jwt.ValidTo;
        }

        private UserProfile? ReadUserProfile(string token)
        {
            if (!_jwtHandler.CanReadToken(token))
                return null;

            var jwt = _jwtHandler.ReadJwtToken(token);

            var idClaim = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)
                       ?? jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub);
            var nameClaim = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)
                        ?? jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.UniqueName);
            var emailClaim = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)
                          ?? jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email);

            if (idClaim == null || !int.TryParse(idClaim.Value, out var userId))
                return null;

            return new UserProfile
            {
                Id = userId,
                Username = nameClaim?.Value ?? string.Empty,
                Email = emailClaim?.Value
            };
        }

        private void SetState(AuthState state)
        {
            if (State == state)
                return;

            State = state;
            StateChanged?.Invoke(this, state);
        }
    }
}
