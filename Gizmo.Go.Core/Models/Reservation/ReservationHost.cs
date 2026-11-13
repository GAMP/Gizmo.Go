namespace Gizmo.Go.Core.Models.Reservation
{
    /// <summary>
    /// A host of a reservation.
    /// </summary>
    public sealed class ReservationHost
    {
        /// <summary>
        /// Gets the host number.
        /// </summary>
        public int HostNumber { get; init; }

        /// <summary>
        /// Gets the host name (e.g. "PC #04").
        /// </summary>
        public string HostName { get; init; } = string.Empty;

        /// <summary>
        /// Gets the host group (zone) name; null when the host has no group.
        /// </summary>
        public string? HostGroupName { get; init; }

        /// <summary>
        /// Gets the status of the host reservation.
        /// </summary>
        public GoReservationHostStatus Status { get; init; }
    }
}
