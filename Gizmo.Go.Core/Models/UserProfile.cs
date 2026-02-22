namespace Gizmo.Go.Core.Models
{
    /// <summary>
    /// Current authenticated user profile.
    /// </summary>
    public sealed class UserProfile
    {
        /// <summary>
        /// Gets or sets user id.
        /// </summary>
        public required int Id { get; init; }

        /// <summary>
        /// Gets or sets username.
        /// </summary>
        public required string Username { get; init; }

        /// <summary>
        /// Gets or sets email.
        /// </summary>
        public string? Email { get; init; }
    }
}
