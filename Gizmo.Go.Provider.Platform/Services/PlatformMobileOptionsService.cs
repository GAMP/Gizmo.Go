using Gizmo.Go.Core.Models.Options;
using Gizmo.Go.Core.Services;

namespace Gizmo.Go.Provider.Platform.Services
{
    internal sealed class PlatformMobileOptionsService : IMobileOptionsService
    {
        public Task<GoMobileOptions> GetAsync(CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();
    }
}
