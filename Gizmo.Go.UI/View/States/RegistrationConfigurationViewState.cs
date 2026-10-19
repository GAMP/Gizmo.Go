using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Go.UI.View.States
{
    /// <summary>
    /// Registration configuration view state.
    /// </summary>
    [Register()]
    public sealed class RegistrationConfigurationViewState : ViewStateBase
    {
        public bool IsEnabled { get; internal set; }

        public bool IsDirectEnabled { get; internal set; }

        public bool IsPasswordRecoveryEnabled { get; internal set; }
    }
}
