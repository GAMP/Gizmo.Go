using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Go.UI.View.States.Pages.Registration
{
    [Register()]
    public sealed class UserRegistrationSuccessViewState : ViewStateBase
    {
        /// <summary>
        /// Gets the masked contact the new account is bound to, empty when unknown.
        /// </summary>
        public string DisplayContact { get; internal set; } = string.Empty;

        public bool IsEmailContact { get; internal set; }
    }
}
