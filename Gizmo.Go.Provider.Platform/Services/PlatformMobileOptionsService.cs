using Gizmo.Go.Core.Models;
using Gizmo.Go.Core.Services;

namespace Gizmo.Go.Provider.Platform.Services
{
    internal sealed class PlatformMobileOptionsService : IMobileOptionsService
    {
        public Task<MobileOptions> GetAsync(CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();
    }
}
