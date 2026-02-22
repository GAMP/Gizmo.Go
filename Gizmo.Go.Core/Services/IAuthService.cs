using Gizmo.Go.Core.Models;

namespace Gizmo.Go.Core.Services
{
    /// <summary>
    /// Authentication service.
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Gets current authentication state.
        /// </summary>
        AuthState State { get; }

        /// <summary>
        /// Gets current authenticated user profile, if any.
        /// </summary>
        UserProfile? CurrentUser { get; }

        /// <summary>
        /// Authenticates with username and password.
        /// </summary>
        Task<AuthResult> LoginAsync(string username, string password, CancellationToken cancellationToken = default);

        /// <summary>
        /// Logs out the current user.
        /// </summary>
        Task LogoutAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets current valid access token, refreshing if needed.
        /// </summary>
        Task<string?> GetTokenAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Raised when authentication state changes.
        /// </summary>
        event EventHandler<AuthState> StateChanged;
    }
}
