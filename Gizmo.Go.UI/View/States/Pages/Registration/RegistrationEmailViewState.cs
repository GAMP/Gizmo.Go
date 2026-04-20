using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Go.UI.View.States.Pages.Registration
{
    [Register()]
    public sealed class RegistrationEmailViewState : ViewStateBase
    {
        public string Email { get; internal set; } = string.Empty;
    }
}
