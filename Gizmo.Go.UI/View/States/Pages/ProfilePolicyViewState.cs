using Gizmo.Go.Core.Models.Agreements;
using Gizmo.UI;
using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Go.UI.View.States.Pages
{
    [Register()]
    public sealed class ProfilePolicyViewState : ViewStateBase
    {
        public ViewBlock<UserAgreement> Agreement { get; } = new();

        /// <summary>
        /// Gets whether the agreement is not in the enabled agreements (the load succeeded without a result).
        /// </summary>
        public bool IsNotFound { get; internal set; }
    }
}
