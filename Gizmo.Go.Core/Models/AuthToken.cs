namespace Gizmo.Go.Core.Models
{
    /// <summary>
    /// Authentication token pair.
    /// </summary>
    public sealed class AuthToken
    {
        /// <summary>
        /// Gets or sets access token.
        /// </summary>
        public required string Token { get; init; }

        /// <summary>
        /// Gets or sets refresh token.
        /// </summary>
        public required string RefreshToken { get; init; }

        /// <summary>
        /// Gets or sets token expiration time (UTC).
        /// </summary>
        public DateTime? ExpiresUtc { get; init; }

        /// <summary>
        /// Gets or sets whether the server requested the user profile step on login ("personal info requested").
        /// </summary>
        public bool IsPersonalInfoRequested { get; init; }
    }
}
