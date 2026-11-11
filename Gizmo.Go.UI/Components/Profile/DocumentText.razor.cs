using Microsoft.AspNetCore.Components;

namespace Gizmo.Go.UI.Components.Profile
{
    /// <summary>
    /// Card with the text of a document.
    /// </summary>
    public partial class DocumentText : ComponentBase
    {
        [Parameter]
        public string? Text { get; set; }

        /// <summary>
        /// Gets or sets whether the text is HTML (a server agreement) or plain text (an app document).
        /// </summary>
        [Parameter]
        public bool IsHtml { get; set; }
    }
}
