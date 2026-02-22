using Gizmo.Go.Core.Models;
using Gizmo.Go.Core.Services;

namespace Gizmo.Go.Provider.Direct.Services
{
    /// <summary>
    /// Direct Gizmo Server branch provider.
    /// </summary>
    internal sealed class DirectBranchProvider : IBranchProvider
    {
        public Task<IEnumerable<Branch>> GetAsync(BranchFilter? filter = null, CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public Task<Branch?> GetByIdAsync(int branchId, CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public Task<IEnumerable<Branch>> GetFavoritesAsync(CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public Task AddFavoriteAsync(int branchId, CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public Task RemoveFavoriteAsync(int branchId, CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();
    }
}
