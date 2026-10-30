
namespace Gizmo.Go.Core.Models.Profile
{
    /// <summary>
    /// Profile values to save; <c>null</c> means unchanged.
    /// </summary>
    public sealed class UserProfileUpdate
    {
        public string? FirstName { get; init; }
        public string? LastName { get; init; }
        public DateTime? BirthDate { get; init; }
        public UserSex? Sex { get; init; }
        public string? Email { get; init; }
        public string? MobilePhone { get; init; }
        public string? Phone { get; init; }
        public string? Address { get; init; }
        public string? City { get; init; }
        public string? PostCode { get; init; }
        public string? Country { get; init; }
    }
}
