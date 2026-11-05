using Gizmo.Go.Core.Models.Ladder;
using Gizmo.Go.Core.Services;
using Gizmo.Go.Provider.Direct.Helpers;
using Gizmo.Go.Provider.Direct.Mappers.Ladder;
using Gizmo.Web.Api.Models;
using Microsoft.Extensions.DependencyInjection;
using LadderClient = Gizmo.Web.Api.User.Clients.AchievementLadderWebApiClient;

namespace Gizmo.Go.Provider.Direct.Services
{
    /// <summary>
    /// Direct Gizmo Server achievement ladder service (api/user/v3/ladder, secured).
    /// </summary>
    internal sealed class DirectLadderService : ILadderService
    {
        private readonly IServiceProvider _serviceProvider;

        public DirectLadderService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<LadderLevel?> GetLevelAsync(CancellationToken cancellationToken = default)
        {
            var client = _serviceProvider.GetRequiredService<LadderClient>();
            var standing = await client.GetStandingAsync(new LadderStandingFilter { Progress = false }, cancellationToken);
            return LadderLevelMapper.Map(standing, BackendUrl.Resolve(_serviceProvider));
        }
    }
}
