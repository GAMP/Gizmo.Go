namespace Gizmo.Go.Core.Models.Reservation
{
    /// <summary>
    /// The nearest reservation of the current user.
    /// </summary>
    public sealed class NearestReservation
    {
        /// <summary>
        /// Gets the reservation id.
        /// </summary>
        public int Id { get; init; }

        /// <summary>
        /// Gets the name of the reserved branch.
        /// </summary>
        public string BranchName { get; init; } = string.Empty;

        /// <summary>
        /// Gets the address of the reserved branch.
        /// </summary>
        public string? BranchAddress { get; init; }

        /// <summary>
        /// Gets the name of the reserved host (e.g. "PC #04"); null when the reservation has no single host.
        /// </summary>
        public string? HostName { get; init; }

        /// <summary>
        /// Gets the local start time.
        /// </summary>
        public DateTime Start { get; init; }

        /// <summary>
        /// Gets the local end time.
        /// </summary>
        public DateTime End { get; init; }

        /// <summary>
        /// Gets the reservation status.
        /// </summary>
        public NearestReservationStatus Status { get; init; }
    }
}
