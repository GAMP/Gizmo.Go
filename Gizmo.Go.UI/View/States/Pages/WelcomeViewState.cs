using Gizmo.UI;
using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Go.UI.View.States
{
    [Register()]
    public sealed class WelcomeViewState : ViewStateBase
    {
        public bool IsRegistrationEnabled { get; internal set; }

        /// <summary>
        /// Gets if the server options could not be loaded (server unreachable or an old server).
        /// </summary>
        public bool HasOptionsError { get; internal set; }
    }
}
