using System.Diagnostics.CodeAnalysis;
using System.Web;
using Gizmo.Go.Core.Models.News;
using Gizmo.Web.Api.Models;

namespace Gizmo.Go.Provider.Direct.Mappers.News
{
    internal static class NewsItemMapper
    {
        internal static NewsItem Map(NewsModel source) =>
            new()
            {
                Id         = source.Id,
                Title      = source.Title,
                Body       = source.Data ?? string.Empty,
                ImageUrl   = ParseImageUrl(source.ThumbnailUrl, source.MediaUrl),
                Date       = source.StartDate,
                BranchName = MapBranchName(source),
            };

        // null means "all clubs" in the UI
        private static string? MapBranchName(NewsModel source)
        {
            if (source.IsAllBranches || source.BranchNames is null)
                return null;

            var names = string.Join(", ", source.BranchNames);
            return names.Length == 0 ? null : names;
        }

        // as the client: an absolute thumbnail, otherwise the YouTube preview of the media;
        // the video id is the "v" query value (watch?v=) or the last segment (embed/{id});
        // a media url without a video id (the site root, a channel) has no preview
        private static string? ParseImageUrl(string? thumbnailUrl, string? mediaUrl)
        {
            if (IsHttpUrl(thumbnailUrl, out _))
                return thumbnailUrl;

            if (!IsHttpUrl(mediaUrl, out var mediaUri) || mediaUri.Host != "www.youtube.com")
                return null;

            var query = HttpUtility.ParseQueryString(mediaUri.Query);
            var videoId = (query["v"] ?? mediaUri.Segments[^1]).Trim('/');

            if (videoId.Length == 0 || videoId.StartsWith('@'))
                return null;

            return $"https://i3.ytimg.com/vi/{videoId}/maxresdefault.jpg";
        }

        // a root-relative path is an absolute file uri on Android and WebAssembly, so the scheme is checked
        private static bool IsHttpUrl(string? url, [NotNullWhen(true)] out Uri? uri) =>
            Uri.TryCreate(url, UriKind.Absolute, out uri) && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
    }
}
