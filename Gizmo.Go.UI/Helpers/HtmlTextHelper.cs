using System.Net;
using System.Text.RegularExpressions;

namespace Gizmo.Go.UI.Helpers
{
    internal static partial class HtmlTextHelper
    {
        [GeneratedRegex("<[^>]*>")]
        private static partial Regex TagRegex();

        [GeneratedRegex(@"\s+")]
        private static partial Regex WhitespaceRegex();

        /// <summary>
        /// Gets the plain text of an HTML fragment: tags removed, entities decoded, whitespace collapsed.
        /// </summary>
        internal static string ToPlainText(string? html)
        {
            if (string.IsNullOrEmpty(html))
                return string.Empty;

            var text = WebUtility.HtmlDecode(TagRegex().Replace(html, " "));
            return WhitespaceRegex().Replace(text, " ").Trim();
        }
    }
}
