using Gizmo.Go.Core.Models.News;
using Gizmo.Go.Core.Services;

namespace Gizmo.Go.Provider.Platform.Services
{
    internal sealed class PlatformNewsService : INewsService
    {
        public Task<IReadOnlyList<NewsItem>> GetListAsync(int? limit = null, CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public Task<NewsItem?> GetAsync(int id, CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();
    }
}
