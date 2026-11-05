using Gizmo.Go.Core.Models.News;

namespace Gizmo.Go.Core.Services
{
    /// <summary>
    /// News.
    /// </summary>
    public interface INewsService
    {
        /// <summary>
        /// Gets news, newest first; <paramref name="limit"/> null returns all.
        /// </summary>
        Task<IReadOnlyList<NewsItem>> GetListAsync(int? limit = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a news item by id; null when it does not exist.
        /// </summary>
        Task<NewsItem?> GetAsync(int id, CancellationToken cancellationToken = default);
    }
}
