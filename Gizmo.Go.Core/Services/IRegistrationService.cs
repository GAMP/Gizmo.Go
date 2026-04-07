
using Gizmo.Go.Core.Models.Registration;
using Gizmo.Web.Api.Models;

namespace Gizmo.Go.Core.Services
{
    public interface IRegistrationService
    {
        Task<IReadOnlyList<VerificationProvider>> GetProvidersAsync(
            CancellationToken cancellationToken = default);

        Task<RegistrationStartResult> StartAsync(
            RegistrationStartRequest request,
            CancellationToken cancellationToken = default);
    }
}
