using Microsoft.AspNetCore.Components;

namespace Gizmo.Go.UI.Components.Profile
{
    /// <summary>
    /// List of documents (policies, app documents): a document icon, the title and a chevron per row.
    /// </summary>
    public partial class DocumentList : ComponentBase
    {
        /// <summary>One row of the list.</summary>
        /// <param name="Id">Document id passed to <see cref="OnSelect"/>.</param>
        /// <param name="Title">Row title.</param>
        public sealed record DocumentListItem(int Id, string Title);

        [Parameter]
        public IReadOnlyList<DocumentListItem> Items { get; set; } = [];

        /// <summary>
        /// Gets or sets the callback of a tapped row with the document id.
        /// </summary>
        [Parameter]
        public EventCallback<int> OnSelect { get; set; }
    }
}
