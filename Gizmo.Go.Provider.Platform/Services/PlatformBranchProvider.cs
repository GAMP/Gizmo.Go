using Gizmo.Go.Core.Models.Branch;
using Gizmo.Go.Core.Services;

namespace Gizmo.Go.Provider.Platform.Services
{
    /// <summary>
    /// Platform API branch provider.
    /// </summary>
    internal sealed class PlatformBranchProvider : IBranchProvider
    {
        public Task<IEnumerable<Branch>> GetAsync(CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public Task<BranchDetails?> GetDetailsAsync(int branchId, CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();
    }
}
