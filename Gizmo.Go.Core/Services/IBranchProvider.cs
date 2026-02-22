using Gizmo.Go.Core.Models;

namespace Gizmo.Go.Core.Services
{
    /// <summary>
    /// Branch provider service.
    /// </summary>
    public interface IBranchProvider
    {
        /// <summary>
        /// Gets branches matching the specified filter.
        /// </summary>
        Task<IEnumerable<Branch>> GetAsync(BranchFilter? filter = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a branch by id.
        /// </summary>
        Task<Branch?> GetByIdAsync(int branchId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets user's favorite branches.
        /// </summary>
        Task<IEnumerable<Branch>> GetFavoritesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a branch to user's favorites.
        /// </summary>
        Task AddFavoriteAsync(int branchId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes a branch from user's favorites.
        /// </summary>
        Task RemoveFavoriteAsync(int branchId, CancellationToken cancellationToken = default);
    }
}
