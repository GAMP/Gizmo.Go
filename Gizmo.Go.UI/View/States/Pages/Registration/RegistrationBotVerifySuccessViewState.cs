using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Go.UI.View.States.Pages.Registration
{
    [Register()]
    public sealed class RegistrationBotVerifySuccessViewState : ViewStateBase
    {
        public string DisplayPhone { get; internal set; } = string.Empty;
    }
}
