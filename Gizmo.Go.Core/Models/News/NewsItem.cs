namespace Gizmo.Go.Core.Models.News
{
    /// <summary>
    /// News item.
    /// </summary>
    public sealed class NewsItem
    {
        /// <summary>
        /// Gets the news id.
        /// </summary>
        public int Id { get; init; }

        /// <summary>
        /// Gets the news title.
        /// </summary>
        public string Title { get; init; } = string.Empty;

        /// <summary>
        /// Gets the news body (HTML).
        /// </summary>
        public string Body { get; init; } = string.Empty;

        /// <summary>
        /// Gets the absolute URL of the news image; null when the news has none.
        /// </summary>
        public string? ImageUrl { get; init; }

        /// <summary>
        /// Gets the publication date; null when the news has no start date.
        /// </summary>
        public DateTime? Date { get; init; }

        /// <summary>
        /// Gets the name of the branch the news belongs to; null for all branches.
        /// </summary>
        public string? BranchName { get; init; }
    }
}
