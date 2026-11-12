using Gizmo.Go.Core.Models.Branch;

namespace Gizmo.Go.Core.Services
{
    /// <summary>
    /// Branch provider service.
    /// </summary>
    public interface IBranchProvider
    {
        /// <summary>
        /// Gets all active branches ordered by name.
        /// </summary>
        Task<IEnumerable<Branch>> GetAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the active branch with its computer counts; null when it does not exist or is not active.
        /// </summary>
        Task<BranchDetails?> GetDetailsAsync(int branchId, CancellationToken cancellationToken = default);
    }
}
