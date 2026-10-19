using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Go.UI.View.States.Pages.Registration
{
    [Register]
    public sealed class UserRegistrationIndexViewState : ValidatingViewStateBase
    {
        public IEnumerable<UserAgreementViewState> UserAgreementStates { get; internal set; } = Enumerable.Empty<UserAgreementViewState>();

        public bool HasError { get; internal set; }

        public string ErrorMessage { get; internal set; } = string.Empty;
    }
}
