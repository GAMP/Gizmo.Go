using Gizmo.Go.Core.Models;

namespace Gizmo.Go.Core.Services
{
    /// <summary>
    /// Platform-agnostic token persistence service.
    /// </summary>
    public interface ITokenStorageService
    {
        /// <summary>
        /// Gets stored token data.
        /// </summary>
        ValueTask<AuthToken?> GetAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Stores token data.
        /// </summary>
        ValueTask SetAsync(AuthToken data, CancellationToken cancellationToken = default);

        /// <summary>
        /// Clears stored token data.
        /// </summary>
        ValueTask ClearAsync(CancellationToken cancellationToken = default);
    }
}
