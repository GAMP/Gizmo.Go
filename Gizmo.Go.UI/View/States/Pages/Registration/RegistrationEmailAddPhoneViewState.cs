using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Go.UI.View.States.Pages.Registration
{
    [Register()]
    public sealed class RegistrationEmailAddPhoneViewState : ViewStateBase
    {
        public string Phone { get; internal set; } = string.Empty;
        public string ConfirmedEmail { get; internal set; } = string.Empty;
    }
}
