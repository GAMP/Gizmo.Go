using Gizmo.Go.Core.Models.Registration;
using Gizmo.Go.Core.Services;

namespace Gizmo.Go.Provider.Platform.Services
{
    internal sealed class PlatformRegistrationService : IRegistrationService
    {
        public Task<IReadOnlyList<RegistrationProvider>> GetProvidersAsync(
            CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public Task<RegistrationStartResult> StartAsync(
            RegistrationStartRequest request,
            CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public Task<RegistrationCompleteResult> CompleteAsync(
            RegistrationCompleteRequest request,
            CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();
    }   
}
