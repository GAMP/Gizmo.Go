using Gizmo.UI;
using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Go.UI.View.States.Pages
{
    [Register()]
    public sealed class ProfileAboutDocumentViewState : ViewStateBase
    {
        public string Title { get; internal set; } = string.Empty;

        public string Text { get; internal set; } = string.Empty;

        /// <summary>
        /// Gets whether the document id is not one of the app documents.
        /// </summary>
        public bool IsNotFound { get; internal set; }
    }
}
