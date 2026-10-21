using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Go.UI.View.States.Pages.PasswordRecovery
{
    [Register()]
    public sealed class PasswordRecoveryAccountViewState : ViewStateBase
    {
        public bool IsFound { get; internal set; }
    }
}
