using Gizmo.Go.Core.Models.Balance;
using Gizmo.Go.Core.Services;

namespace Gizmo.Go.Provider.Platform.Services
{
    internal sealed class PlatformUserBalanceService : IUserBalanceService
    {
        public Task<UserBalance> GetAsync(CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();
    }
}
