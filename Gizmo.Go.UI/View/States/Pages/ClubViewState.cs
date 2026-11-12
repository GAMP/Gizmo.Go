using Gizmo.Go.Core.Models.Branch;
using Gizmo.UI;
using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Go.UI.View.States.Pages
{
    [Register()]
    public sealed class ClubViewState : ViewStateBase
    {
        public ViewBlock<BranchDetails> Details { get; } = new();

        /// <summary>
        /// Gets whether the club does not exist or is not active (the load succeeded without a result).
        /// </summary>
        public bool IsNotFound { get; internal set; }

        /// <summary>
        /// Gets whether the working hours of the whole week are shown.
        /// </summary>
        public bool IsScheduleExpanded { get; internal set; }
    }
}
