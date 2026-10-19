using Gizmo.Go.Core.Models.Registration;
using Gizmo.Go.Core.Services;

namespace Gizmo.Go.Provider.Platform.Services
{
    internal sealed class PlatformRegistrationOptionsService : IRegistrationOptionsService
    {
        public Task<RegistrationOptions> GetAsync(CancellationToken ct = default) =>
            throw new NotImplementedException();
    }
}
