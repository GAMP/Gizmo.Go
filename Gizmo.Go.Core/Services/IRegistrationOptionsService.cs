using Gizmo.Go.Core.Models.Registration;

namespace Gizmo.Go.Core.Services
{
    /// <summary>
    /// Registration and password recovery availability, and the logo of the auth screens.
    /// </summary>
    public interface IRegistrationOptionsService
    {
        Task<RegistrationOptions> GetAsync(CancellationToken ct = default);
    }
}
