using Gizmo.Go.Core.Models.Registration;

namespace Gizmo.Go.Core.Services
{
    public interface IRegistrationService
    {
        Task<IReadOnlyList<RegistrationProvider>> GetProvidersAsync(
            CancellationToken cancellationToken = default);

        Task<RegistrationStartResult> StartAsync(
            RegistrationStartRequest request,
            CancellationToken cancellationToken = default);

        Task<RegistrationCompleteResult> CompleteAsync(
            RegistrationCompleteRequest request,
            CancellationToken cancellationToken = default);
    }
}
