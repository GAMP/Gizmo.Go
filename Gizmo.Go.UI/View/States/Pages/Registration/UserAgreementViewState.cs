using Gizmo.Go.Core.Models.Registration;
using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Go.UI.View.States.Pages.Registration
{
    [Register(Scope = RegisterScope.Transient)]
    public sealed class UserAgreementViewState : ViewStateBase
    {
        public int Id { get; internal set; }

        public string? Name { get; internal set; }

        public string? Agreement { get; internal set; }

        public bool IsRejectable { get; internal set; }

        public UserAgreementAcceptState AcceptState { get; internal set; }
    }
}
