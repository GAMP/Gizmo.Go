using Gizmo.Go.Core.Models;
using Gizmo.Go.Core.Services;
using Gizmo.Go.Provider.Direct.Demo;

namespace Gizmo.Go.Provider.Direct.Services
{
    /// <summary>
    /// Direct Gizmo Server branch provider.
    /// </summary>
    internal sealed class DirectBranchProvider : IBranchProvider
    {
        // demo data until GET api/user/v3/branches?city= exists
        public async Task<IEnumerable<Branch>> GetAsync(BranchFilter? filter = null, CancellationToken cancellationToken = default)
        {
            await Task.Delay(HomeDemoData.Delay, cancellationToken);

            var city = filter?.City?.Trim();

            return HomeDemoData.Branches
                .Where(branch => string.IsNullOrEmpty(city)
                    || string.Equals(branch.City?.Trim(), city, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        public Task<Branch?> GetByIdAsync(int branchId, CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public Task<IEnumerable<Branch>> GetFavoritesAsync(CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public Task AddFavoriteAsync(int branchId, CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public Task RemoveFavoriteAsync(int branchId, CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        // demo data until GET api/user/v3/branches/cities exists
        public async Task<IReadOnlyList<string>> GetCitiesAsync(CancellationToken cancellationToken = default)
        {
            await Task.Delay(HomeDemoData.Delay, cancellationToken);

            return HomeDemoData.Branches
                .Select(branch => branch.City?.Trim())
                .Where(city => !string.IsNullOrEmpty(city))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Cast<string>()
                .ToList();
        }
    }
}
