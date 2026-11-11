using Gizmo.Go.Core.Models;
using Gizmo.Go.Core.Models.Profile;

namespace Gizmo.Go.Core.Services
{
    /// <summary>
    /// Profile of the current user.
    /// </summary>
    public interface IUserProfileService
    {
        Task<UserProfileDetails> GetAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the fields required by the group of the current user.
        /// </summary>
        Task<RequiredUserInfo> GetRequiredInfoAsync(CancellationToken cancellationToken = default);

        Task<UserProfileUpdateCode> UpdateAsync(UserProfileUpdate update, CancellationToken cancellationToken = default);

        /// <summary>
        /// Changes the password of the current user. Network and other unexpected errors are thrown to the caller.
        /// </summary>
        /// <param name="currentPassword">Current password.</param>
        /// <param name="newPassword">New password.</param>
        Task<UserPasswordChangeCode> ChangePasswordAsync(string currentPassword, string newPassword, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes the account of the current user (soft delete on the server). Failures are thrown to the caller.
        /// </summary>
        Task DeleteAsync(CancellationToken cancellationToken = default);
    }
}
