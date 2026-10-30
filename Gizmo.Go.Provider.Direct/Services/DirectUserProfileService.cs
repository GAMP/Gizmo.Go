using Gizmo.Go.Core.Models;
using Gizmo.Go.Core.Models.Profile;
using Gizmo.Go.Core.Services;
using Gizmo.Go.Provider.Direct.Mappers.Common;
using Gizmo.Go.Provider.Direct.Mappers.Profile;
using Gizmo.Server.Exceptions;
using Gizmo.Web.Api.Clients;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProfileClient = Gizmo.Web.Api.User.Clients.ProfileWebApiClient;

namespace Gizmo.Go.Provider.Direct.Services
{
    /// <summary>
    /// Direct Gizmo Server current user profile service (api/user/v3/profile).
    /// </summary>
    internal sealed class DirectUserProfileService : IUserProfileService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DirectUserProfileService> _logger;

        public DirectUserProfileService(IServiceProvider serviceProvider, ILogger<DirectUserProfileService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task<UserProfileDetails> GetAsync(CancellationToken cancellationToken = default)
        {
            var client = _serviceProvider.GetRequiredService<ProfileClient>();
            return UserProfileMapper.Map(await client.GetAsync(cancellationToken));
        }

        public async Task<RequiredUserInfo> GetRequiredInfoAsync(CancellationToken cancellationToken = default)
        {
            var client = _serviceProvider.GetRequiredService<ProfileClient>();
            return RequiredUserInfoMapper.Map(await client.GetRequiredInfoAsync(cancellationToken));
        }

        public async Task<UserProfileUpdateCode> UpdateAsync(UserProfileUpdate update, CancellationToken cancellationToken = default)
        {
            try
            {
                var client = _serviceProvider.GetRequiredService<ProfileClient>();
                await client.UpdateAsync(UserProfileMapper.Map(update), cancellationToken);
                return UserProfileUpdateCode.Success;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex) when (ex.IsExceptionCode(ExceptionCode.NonUniqueEntityValue))
            {
                _logger.LogWarning(ex, "Profile update rejected: non-unique value.");
                return UserProfileUpdateCode.NonUniqueValue;
            }
            catch (Exception ex) when (ex.IsExceptionCode(ExceptionCode.ValidationError) || ex.IsExceptionCode(ExceptionCode.InvalidProperty))
            {
                _logger.LogWarning(ex, "Profile update rejected: invalid input.");
                return UserProfileUpdateCode.InvalidInput;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Profile update failed.");
                return UserProfileUpdateCode.Failure;
            }
        }
    }
}
