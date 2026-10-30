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
    }
}
