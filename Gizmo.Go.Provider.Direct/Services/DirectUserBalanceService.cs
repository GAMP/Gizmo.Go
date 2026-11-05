using Gizmo.Go.Core.Models.Balance;
using Gizmo.Go.Core.Services;
using Gizmo.Go.Provider.Direct.Mappers.Balance;
using Microsoft.Extensions.DependencyInjection;
using BalanceClient = Gizmo.Web.Api.User.Clients.BalanceWebApiClient;

namespace Gizmo.Go.Provider.Direct.Services
{
    /// <summary>
    /// Direct Gizmo Server balance of the current user (api/user/v3/balance, secured).
    /// </summary>
    internal sealed class DirectUserBalanceService : IUserBalanceService
    {
        private readonly IServiceProvider _serviceProvider;

        public DirectUserBalanceService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<UserBalance> GetAsync(CancellationToken cancellationToken = default)
        {
            var client = _serviceProvider.GetRequiredService<BalanceClient>();
            var model = await client.GetAsync(cancellationToken);
            return UserBalanceMapper.Map(model);
        }
    }
}
