using Gizmo.Go.Core.Models.Booking;

namespace Gizmo.Go.UI.View.States.Pages
{
    /// <summary>
    /// A tab of the PC selection: a hall of the club, the hosts outside the halls or all hosts of a club without halls.
    /// </summary>
    public sealed class BookingTab
    {
        /// <summary>
        /// Gets the tab key: the hall id, <see cref="OtherKey"/> or <see cref="AllKey"/>.
        /// </summary>
        public int Key { get; init; }

        /// <summary>
        /// Gets the tab name.
        /// </summary>
        public string Name { get; init; } = string.Empty;

        /// <summary>
        /// Gets the hosts of the tab with their place on the map.
        /// </summary>
        public IReadOnlyList<BookingTabHost> Hosts { get; init; } = [];

        /// <summary>
        /// Key of the tab with the hosts that are in no hall.
        /// </summary>
        public const int OtherKey = 0;

        /// <summary>
        /// Key of the only tab of a club without halls.
        /// </summary>
        public const int AllKey = -1;
    }

    /// <summary>
    /// A host of a tab with its place on the map.
    /// </summary>
    public sealed class BookingTabHost
    {
        /// <summary>
        /// Gets the host.
        /// </summary>
        public required BookingHost Host { get; init; }

        /// <summary>
        /// Gets the map row, from 0.
        /// </summary>
        public int Row { get; init; }

        /// <summary>
        /// Gets the map column, from 0.
        /// </summary>
        public int Column { get; init; }
    }

    /// <summary>
    /// Which part of the cost the user pays when booking.
    /// </summary>
    public enum BookingAmountKind
    {
        /// <summary>
        /// The whole total.
        /// </summary>
        Full,

        /// <summary>
        /// The minimum payment of the offer (may be 0).
        /// </summary>
        Minimum
    }
}
