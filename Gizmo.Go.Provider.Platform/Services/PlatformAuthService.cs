using Gizmo.Go.Core.Models;
using Gizmo.Go.Core.Services;

namespace Gizmo.Go.Provider.Platform.Services
{
    /// <summary>
    /// Platform API authentication service.
    /// </summary>
    internal sealed class PlatformAuthService : IAuthService
    {
        public AuthState State => throw new NotImplementedException();

        public UserProfile? CurrentUser => throw new NotImplementedException();

        public event EventHandler<AuthState>? StateChanged;

        public Task<string?> GetTokenAsync(CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public Task<AuthResult> LoginAsync(string username, string password, CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public Task LogoutAsync(CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();
    }
}
