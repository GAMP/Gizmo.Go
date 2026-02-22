using Gizmo.Go.Core.Models;
using Gizmo.Go.Core.Services;

namespace Gizmo.Go.Provider.Platform.Services
{
    /// <summary>
    /// Platform API branch provider.
    /// </summary>
    internal sealed class PlatformBranchProvider : IBranchProvider
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
