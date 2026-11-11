using Gizmo.Go.Core.Models;
using Gizmo.Go.Core.Models.Profile;
using Gizmo.Go.Core.Services;

namespace Gizmo.Go.Provider.Platform.Services
{
    internal sealed class PlatformUserProfileService : IUserProfileService
    {
        public Task<UserProfileDetails> GetAsync(CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public Task<RequiredUserInfo> GetRequiredInfoAsync(CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public Task<UserProfileUpdateCode> UpdateAsync(UserProfileUpdate update, CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public Task<UserPasswordChangeCode> ChangePasswordAsync(string currentPassword, string newPassword, CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public Task DeleteAsync(CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();
    }
}
