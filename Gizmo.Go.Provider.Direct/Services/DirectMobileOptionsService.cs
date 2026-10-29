using Gizmo.Go.Core.Models;
using Gizmo.Go.Core.Services;
using Gizmo.Go.Provider.Direct.Helpers;
using Gizmo.Go.Provider.Direct.Mappers.Common;
using Microsoft.Extensions.DependencyInjection;
using OptionsClient = Gizmo.Web.Api.User.Clients.OptionsWebApiClient;

namespace Gizmo.Go.Provider.Direct.Services
{
    /// <summary>
    /// Direct Gizmo Server mobile options service (api/user/v3/options/mobile).
    /// </summary>
    internal sealed class DirectMobileOptionsService : IMobileOptionsService
    {
        private readonly IServiceProvider _serviceProvider;
        private MobileOptions? _cached;

        public DirectMobileOptionsService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<MobileOptions> GetAsync(CancellationToken cancellationToken = default)
        {
            if (_cached is not null)
                return _cached;

            var client = _serviceProvider.GetRequiredService<OptionsClient>();
            var model = await client.GetMobileAsync(cancellationToken);

            // same base as the api clients
            _cached = MobileOptionsMapper.Map(model, BackendUrl.Resolve(_serviceProvider));
            return _cached;
        }
    }
}
