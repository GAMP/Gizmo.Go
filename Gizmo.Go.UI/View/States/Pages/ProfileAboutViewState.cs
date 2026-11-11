using Gizmo.UI;
using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Go.UI.View.States.Pages
{
    [Register()]
    public sealed class ProfileAboutViewState : ViewStateBase
    {
        /// <summary>
        /// Gets the app version (the version of Gizmo.Go.UI without the build metadata).
        /// </summary>
        public string Version { get; internal set; } = string.Empty;
    }
}
