using Gizmo.Go.Core.Models.Reservation;
using Gizmo.UI;
using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Go.UI.View.States.Pages
{
    [Register()]
    public sealed class ReservationsViewState : ViewStateBase
    {
        /// <summary>
        /// Gets the current and upcoming reservations, start ascending.
        /// </summary>
        public ViewBlock<IReadOnlyList<Reservation>> Current { get; } = new();

        /// <summary>
        /// Gets the latest completed reservations, newest first.
        /// </summary>
        public ViewBlock<IReadOnlyList<Reservation>> Completed { get; } = new();

        public bool IsCompletedCollapsed { get; internal set; }

        /// <summary>
        /// Gets the ids of the reservations with expanded details.
        /// </summary>
        public IReadOnlySet<int> ExpandedIds => ExpandedIdSet;

        internal HashSet<int> ExpandedIdSet { get; } = [];

        /// <summary>
        /// Gets the id of the reservation whose cancellation waits for the confirmation.
        /// </summary>
        public int? ConfirmingCancelId { get; internal set; }

        public bool IsCanceling { get; internal set; }
    }
}
