using Gizmo.Go.Core.Models;

namespace Gizmo.Go.Core.Services
{
    /// <summary>
    /// Mobile app options provided by the server before user login.
    /// </summary>
    public interface IMobileOptionsService
    {
        /// <summary>
        /// Gets the options. A successful result is cached for the app run; failures are thrown to the caller.
        /// </summary>
        Task<MobileOptions> GetAsync(CancellationToken cancellationToken = default);
    }
}
