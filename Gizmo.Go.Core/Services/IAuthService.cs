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
        /// Gets whether the server requested the profile step for the current session.
        /// Stored with the tokens until <see cref="ClearPersonalInfoRequestedAsync"/>.
        /// </summary>
        bool IsPersonalInfoRequested { get; }

        /// <summary>
        /// Authenticates with username and password.
        /// </summary>
        Task<AuthResult> LoginAsync(string username, string password, CancellationToken cancellationToken = default);

        /// <summary>
        /// Clears the stored "personal info requested" flag after the profile step is completed.
        /// </summary>
        Task ClearPersonalInfoRequestedAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Logs out the current user.
        /// </summary>
        Task LogoutAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets current valid access token, refreshing if needed.
        /// </summary>
        Task<string?> GetTokenAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Attempts to restore a previously persisted session from token storage.
        /// </summary>
        Task TryRestoreSessionAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Raised when authentication state changes.
        /// </summary>
        event EventHandler<AuthState> StateChanged;
    }
}
