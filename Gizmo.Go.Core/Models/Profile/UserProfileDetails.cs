
namespace Gizmo.Go.Core.Models.Profile
{
    /// <summary>
    /// Profile fields of the current user.
    /// </summary>
    public sealed class UserProfileDetails
    {
        /// <summary>
        /// Gets the username (nickname).
        /// </summary>
        public string Username { get; init; } = string.Empty;
        public string? FirstName { get; init; }
        public string? LastName { get; init; }
        public DateTime? BirthDate { get; init; }
        public UserSex Sex { get; init; }
        public string? Email { get; init; }
        public string? MobilePhone { get; init; }
        public string? Phone { get; init; }
        public string? Address { get; init; }
        public string? City { get; init; }
        public string? PostCode { get; init; }
        public string? Country { get; init; }

        /// <summary>
        /// Gets the registration date; null until the server provides it.
        /// </summary>
        public DateTime? RegisteredAt { get; init; }
    }
}
