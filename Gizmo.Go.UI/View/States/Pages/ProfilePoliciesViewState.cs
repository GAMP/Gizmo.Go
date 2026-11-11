using Gizmo.Go.Core.Models.Agreements;
using Gizmo.UI;
using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Go.UI.View.States.Pages
{
    [Register()]
    public sealed class ProfilePoliciesViewState : ViewStateBase
    {
        public ViewBlock<IReadOnlyList<UserAgreement>> Agreements { get; } = new();
    }
}
