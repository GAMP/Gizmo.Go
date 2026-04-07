using Gizmo.Go.Core.Models.Registration;
using Gizmo.Go.Core.Services;
using Gizmo.Web.Api.Models;

namespace Gizmo.Go.Provider.Platform.Services
{
    internal sealed class PlatformRegistrationService : IRegistrationService
    {
        public Task<IReadOnlyList<VerificationProvider>> GetProvidersAsync(
            CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public Task<RegistrationStartResult> StartAsync(
            RegistrationStartRequest request,
            CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();
    }
}
