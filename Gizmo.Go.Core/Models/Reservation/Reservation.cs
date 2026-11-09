namespace Gizmo.Go.Core.Models.Reservation
{
    /// <summary>
    /// A reservation of the current user, as the server returns it.
    /// </summary>
    public sealed class Reservation
    {
        /// <summary>
        /// Gets the reservation id.
        /// </summary>
        public int Id { get; init; }

        /// <summary>
        /// Gets the reservation start (UTC).
        /// </summary>
        public DateTime Date { get; init; }

        /// <summary>
        /// Gets the reservation start in the wall-clock time of the club.
        /// </summary>
        public DateTime BranchDate { get; init; }

        /// <summary>
        /// Gets the duration in minutes.
        /// </summary>
        public int Duration { get; init; }

        /// <summary>
        /// Gets the reservation status.
        /// </summary>
        public ReservationStatus Status { get; init; }

        /// <summary>
        /// Gets the branch (club) id.
        /// </summary>
        public int BranchId { get; init; }

        /// <summary>
        /// Gets the branch (club) name.
        /// </summary>
        public string BranchName { get; init; } = string.Empty;

        /// <summary>
        /// Gets the branch (club) address.
        /// </summary>
        public string? BranchAddress { get; init; }

        /// <summary>
        /// Gets a value indicating whether the user is the owner of the reservation (otherwise a member).
        /// </summary>
        public bool IsOwner { get; init; }

        /// <summary>
        /// Gets the reservation PIN.
        /// </summary>
        public string? Pin { get; init; }

        /// <summary>
        /// Gets the payment status.
        /// </summary>
        public ReservationPaymentStatus PaymentStatus { get; init; }

        /// <summary>
        /// Gets the total of the reservation invoices.
        /// </summary>
        public decimal Total { get; init; }

        /// <summary>
        /// Gets the outstanding amount of the reservation invoices.
        /// </summary>
        public decimal Outstanding { get; init; }

        /// <summary>
        /// Gets the cancellation grace period in minutes; the refund cutoff is the start minus this value,
        /// or the start when null.
        /// </summary>
        public int? CancellationGracePeriod { get; init; }

        /// <summary>
        /// Gets the hosts of the reservation.
        /// </summary>
        public IReadOnlyList<ReservationHost> Hosts { get; init; } = [];
    }
}
