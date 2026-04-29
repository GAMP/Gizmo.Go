using Gizmo.Go.Core.Models;
using Gizmo.Go.Core.Services;

namespace Gizmo.Go.Provider.Platform.Services;

/// <summary>
/// Stub auth service for local UI development.
/// Always reports Unauthenticated state. Registration flow does not require login.
/// </summary>
internal sealed class StubAuthService : IAuthService
{
    public AuthState State => AuthState.Unauthenticated;

    public UserProfile? CurrentUser => null;

    public event EventHandler<AuthState>? StateChanged;

    public Task<string?> GetTokenAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult<string?>(null);

    public Task<AuthResult> LoginAsync(string username, string password, CancellationToken cancellationToken = default) =>
        Task.FromResult(AuthResult.Failed(AuthErrorCode.Unexpected));

    public Task LogoutAsync(CancellationToken cancellationToken = default) =>
        Task.CompletedTask;

    public Task TryRestoreSessionAsync(CancellationToken cancellationToken = default) =>
        Task.CompletedTask;
}
